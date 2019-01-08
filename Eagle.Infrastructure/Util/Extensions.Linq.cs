using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Util
{
    /// <summary>
    /// 系统扩展 - Linq
    /// </summary>
    public static partial class Extensions
    {
        public static void Every<T>(this IEnumerable<T> enumerable, int everyIndex, Action<IEnumerable<T>> action)
        {
            List<T> list = enumerable.ToList();
            if (list.Count == 0)
                return;
            int index = 0;
            do
            {
                T[] everyArray;
                if (index + everyIndex > list.Count)
                {
                    everyArray = list.GetRange(index, list.Count - index).ToArray<T>();
                }
                else
                {
                    everyArray = list.GetRange(index, everyIndex).ToArray<T>();
                }
                action(everyArray);
                index += everyArray.Length;
            } while (index < list.Count);
        }

        public static T BuildObjectFromRow<T, TA>(this IEnumerable<TA> enumerable, string nameColumn, string valueColumn)
            where T : class, new()
        {
            if (enumerable == null || !enumerable.Any())
                return null;
            var obj = new T();
            var properties = obj.GetType().GetProperties();

            var propertiesTA = typeof(TA).GetProperties();
            var nameProperty = propertiesTA.First(x => x.Name.ToLower() == nameColumn.ToLower());
            var valueProperty = propertiesTA.First(x => x.Name.ToLower() == valueColumn.ToLower());

            foreach (var propertyInfo in properties)
            {
                propertyInfo.SetValue(obj,
                                      FindValueFromArray(enumerable, nameProperty, valueProperty, propertyInfo.Name,
                                                         propertyInfo.PropertyType));
            }
            return obj;
        }

        private static object FindValueFromArray<TA>(IEnumerable<TA> enumerable, PropertyInfo nameProperty,
                                                     PropertyInfo valueProperty, string name, Type convertType)
        {
            foreach (var row in enumerable)
            {
                if (nameProperty.GetValue(row).ToString().ToLower() == name.ToLower())
                {
                    return Convert.ChangeType(valueProperty.GetValue(row), convertType);
                }
            }
            return null;
        }        
        
        public static List<T> TakeFrom<T>(this List<T> list, Func<T, bool> action)
        {
            var removelist = new List<T>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (action(list[i]))
                {
                    removelist.Add(list[i]);
                    list.RemoveAt(i);
                }
            }
            return removelist;
        }

        public static void SafeAddRange<T>(this List<T> list, IEnumerable<T> addrange)
        {
            if (addrange != null && addrange.Count() > 0)
            {
                list.AddRange(addrange);
            }
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> enumerable, params string[] propertyName)
        {
            List<string> propertyNameList = new List<string>();
            if (propertyName != null)
                propertyNameList.AddRange(propertyName);
            var list = enumerable.ToList();
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (propertyNameList.Count == 0)
                    {
                        result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else
                    {
                        if (propertyNameList.Contains(pi.Name))
                            result.Columns.Add(pi.Name, pi.PropertyType);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (propertyNameList.Count == 0)
                        {
                            object obj = pi.GetValue(list[i], null);
                            tempList.Add(obj);
                        }
                        else
                        {
                            if (propertyNameList.Contains(pi.Name))
                            {
                                object obj = pi.GetValue(list[i], null);
                                tempList.Add(obj);
                            }
                        }
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }

        public static Dictionary<TKey, TValue> ToSafeDictionary<T, TKey, TValue>(this IEnumerable<T> enumerable,
                                                                                 Func<T, TKey> key,
                                                                                 Func<T, TValue> value, IEqualityComparer<TKey> comparer = null)
        {
            var d = comparer != null ? new Dictionary<TKey, TValue>(comparer) : new Dictionary<TKey, TValue>();
            foreach (var e in enumerable)
            {
                d.SafeAdd(key(e), value(e));
            }
            return d;
        }

        public static Dictionary<TKey, T> ToSafeDictionary<T, TKey>(this IEnumerable<T> enumerable,
                                                                                 Func<T, TKey> key, IEqualityComparer<TKey> comparer = null)
        {
            var d = comparer != null ? new Dictionary<TKey, T>(comparer) : new Dictionary<TKey, T>();
            foreach (var e in enumerable)
            {
                d.SafeAdd(key(e), e);
            }
            return d;
        }

        public static HashSet<TKey> ToSafeHashSet<T, TKey>(this IEnumerable<T> enumerable,
                                                           Func<T, TKey> key)
        {
            HashSet<TKey> hashSet = new HashSet<TKey>();
            foreach (var e in enumerable)
            {
                hashSet.SafeAdd(key(e));
            }
            return hashSet;
        }

        public static void SafeAdd<T>(this List<T> list, T entity) where T : class
        {
            if (entity != null)
            {
                list.Add(entity);
            }
        }
    }
}
