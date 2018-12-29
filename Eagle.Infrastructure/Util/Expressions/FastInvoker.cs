using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Util.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastInvoker<T>
    {
        [ThreadStatic]
        static FastInvoker<T> _current;

        FastInvoker()
        {
        }

        public static FastInvoker<T> Current
        {
            get
            {
                if (_current == null)
                    _current = new FastInvoker<T>();

                return _current;
            }
        }

        readonly Dictionary<int, Action<T, object[]>> _withArgs = new Dictionary<int, Action<T, object[]>>();

        public void FastInvoke(T target, Type[] genericTypes, Expression<Action<T>> expression, params object[] args)
        {
            var call = expression.Body as MethodCallExpression;
            if (call == null)
                throw new ArgumentException("Only method call expressions are supported.", "expression");

            MethodInfo method = call.Method;

            int key = GetArgumentHashCode(61 * method.GetHashCode(), genericTypes, args);

            Action<T, object[]> invoker = GetInvoker(key, () =>
            {
                if (method.IsGenericMethod)
                    return method.GetGenericMethodDefinition().ToSpecializedMethod(genericTypes, args);

                return method.ToSpecializedMethod(genericTypes, args);
            }, args);

            invoker(target, args);
        }
        
        Action<T, object[]> GetInvoker(int key, Func<MethodInfo> getMethodInfo, object[] args)
        {
            Action<T, object[]> result;
            if (!_withArgs.TryGetValue(key, out result))
            {
                MethodInfo method = getMethodInfo();

                ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
                ParameterExpression argsParameter = Expression.Parameter(typeof(object[]), "args");

                Func<ParameterInfo, int, Expression> converter = (parameter, index) =>
                {
                    BinaryExpression arrayExpression = Expression.ArrayIndex(argsParameter, Expression.Constant(index));

                    if (parameter.ParameterType.IsValueType)
                        return Expression.Convert(arrayExpression, parameter.ParameterType);

                    return Expression.TypeAs(arrayExpression, parameter.ParameterType);
                };
                Expression[] parameters = method.GetParameters().Select(converter).ToArray();

                MethodCallExpression call = Expression.Call(instanceParameter, method, parameters);

                result = Expression.Lambda<Action<T, object[]>>(call, new[] { instanceParameter, argsParameter }).Compile();
                _withArgs[key] = result;
                return result;
            }
            return result;
        }

        private static int GetArgumentHashCode(int seed, Type[] genericTypes, object[] args)
        {
            int key = seed;
            for (int i = 0; i < genericTypes.Length; i++)
                key ^= genericTypes[i] == null ? 27 * i : genericTypes[i].GetHashCode() * 101 << i;
            for (int i = 0; i < args.Length; i++)
                key ^= args[i] == null ? 31 * i : args[i].GetType().GetHashCode() << i;
            return key;
        }
    }

    public static class FastInvokerExtension
    {
        public static MethodInfo ToSpecializedMethod(this MethodInfo method, Type[] genericTypes, object[] args)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            if (!method.IsGenericMethod)
                return method;

            if (genericTypes == null)
                throw new ArgumentNullException(nameof(genericTypes));
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            Type[] arguments = method.GetGenericArguments()
                .ApplyGenericTypesToArguments(genericTypes);

            arguments = GetGenericTypesFromArguments(method.GetParameters(), arguments, args);

            method = method.MakeGenericMethod(arguments);

            return method;
        }

        private static Type[] GetGenericTypesFromArguments(ParameterInfo[] parameterInfos, Type[] arguments, object[] args)
        {
            var parameters = parameterInfos
                .Merge(args, (x, y) => new { Parameter = x, Argument = y });

            for (int i = 0; i < arguments.Length; i++)
            {
                Type argumentType = arguments[i];

                if (!argumentType.IsGenericParameter)
                    continue;

                parameters
                    .Where(arg => arg.Parameter.ParameterType == argumentType && arg.Argument != null)
                    .Select(arg => arg.Argument.GetType())
                    .Each(type =>
                    {
                        arguments[i] = type;

                        var more = argumentType.GetGenericParameterConstraints()
                            .Where(x => x.IsGenericType)
                            .Where(x => type.Implements(x.GetGenericTypeDefinition()))
                            .SelectMany(x => x.GetGenericArguments()
                                                .Merge(type.GetGenericTypeDeclarations(x.GetGenericTypeDefinition()), (c, a) => new { Argument = c, Type = a }));

                        foreach (var next in more)
                        {
                            for (int j = 0; j < arguments.Length; j++)
                            {
                                if (arguments[j] == next.Argument)
                                    arguments[j] = next.Type;
                            }
                        }
                    });

                foreach (var parameter in parameters.Where(x => x.Parameter.ParameterType.IsGenericType && x.Argument != null))
                {
                    var definition = parameter.Parameter.ParameterType.GetGenericTypeDefinition();
                    var declaredTypesForGeneric = parameter.Argument.GetType().GetGenericTypeDeclarations(definition);

                    var mergeds = parameter.Parameter.ParameterType.GetGenericArguments()
                        .Merge(declaredTypesForGeneric, (p, a) => new { ParameterType = p, ArgumentType = a });

                    foreach (var merged in mergeds)
                    {
                        for (int j = 0; j < arguments.Length; j++)
                        {
                            if (arguments[j] == merged.ParameterType)
                                arguments[j] = merged.ArgumentType;
                        }
                    }
                }
            }

            return arguments;
        }

        /// <summary>
		///   Checks if a type implements the specified interface
		/// </summary>
		/// <param name = "objectType">The type to check</param>
		/// <param name = "interfaceType">The interface type (can be generic, either specific or open)</param>
		/// <returns>True if the interface is implemented by the type, otherwise false</returns>
		public static bool Implements(this Type objectType, Type interfaceType)
        {
            if (objectType == null)
                throw new ArgumentNullException(nameof(objectType));
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            //			Guard.IsTrue(x => x.IsInterface, interfaceType, "interfaceType", "Must be an interface");

            if (interfaceType.IsGenericTypeDefinition)
                return objectType.ImplementsGeneric(interfaceType);

            return interfaceType.IsAssignableFrom(objectType);
        }

        /// <summary>
		///   Checks if a type implements an open generic at any level of the inheritance chain, including all
		///   base classes
		/// </summary>
		/// <param name = "objectType">The type to check</param>
		/// <param name = "interfaceType">The interface type (must be a generic type definition)</param>
		/// <returns>True if the interface is implemented by the type, otherwise false</returns>
		public static bool ImplementsGeneric(this Type objectType, Type interfaceType)
        {
            Type matchedType;
            return objectType.ImplementsGeneric(interfaceType, out matchedType);
        }

        /// <summary>
		///   Checks if a type implements an open generic at any level of the inheritance chain, including all
		///   base classes
		/// </summary>
		/// <param name = "objectType">The type to check</param>
		/// <param name = "interfaceType">The interface type (must be a generic type definition)</param>
		/// <param name = "matchedType">The matching type that was found for the interface type</param>
		/// <returns>True if the interface is implemented by the type, otherwise false</returns>
		public static bool ImplementsGeneric(this Type objectType, Type interfaceType, out Type matchedType)
        {
            Guard.AgainstNull(objectType);
            Guard.AgainstNull(interfaceType);
            Guard.IsTrue(x => x.IsGenericType, interfaceType, "interfaceType", "Must be a generic type");
            Guard.IsTrue(x => x.IsGenericTypeDefinition, interfaceType, "interfaceType", "Must be a generic type definition");

            matchedType = null;

            if (interfaceType.IsInterface)
            {
                matchedType = objectType.GetInterfaces()
                    .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType)
                    .FirstOrDefault();
                if (matchedType != null)
                    return true;
            }

            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == interfaceType)
            {
                matchedType = objectType;
                return true;
            }

            Type baseType = objectType.BaseType;
            if (baseType == null)
                return false;

            return baseType.ImplementsGeneric(interfaceType, out matchedType);
        }

        public static IEnumerable<TResult> Merge<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first));
            if (second == null)
                throw new ArgumentNullException(nameof(second));
            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            using (var e1 = first.GetEnumerator())
            using (var e2 = second.GetEnumerator())
                while (e1.MoveNext())
                {
                    if (!e2.MoveNext())
                        yield break;

                    yield return resultSelector(e1.Current, e2.Current);
                }
        }

        /// <summary>
		/// Enumerates a collection, calling the specified action for each entry in the collection
		/// </summary>
		/// <typeparam name="T">The type of the enumeration</typeparam>
		/// <param name="collection">The collection to enumerate</param>
		/// <param name="callback">The action to call for each entry in the collection</param>
		/// <returns>The collection that was enumerated</returns>
		public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> callback)
        {
            foreach (T item in collection)
            {
                callback(item);
            }

            return collection;
        }

        private static Type[] ApplyGenericTypesToArguments(this Type[] arguments, Type[] genericTypes)
        {
            for (int i = 0; i < arguments.Length && i < genericTypes.Length; i++)
            {
                if (genericTypes[i] != null)
                    arguments[i] = genericTypes[i];
            }

            return arguments;
        }

        public static IEnumerable<Type> GetGenericTypeDeclarations(this Type objectType, Type genericType)
        {
            Guard.AgainstNull(objectType, "objectType");
            Guard.AgainstNull(genericType, "genericType");
            Guard.IsTrue(x => x.IsGenericTypeDefinition, genericType, "genericType", "Must be an open generic type");

            Type matchedType;
            if (objectType.ImplementsGeneric(genericType, out matchedType))
            {
                foreach (Type argument in matchedType.GetGenericArguments())
                {
                    yield return argument;
                }
            }
        }
    }
}
