using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Util.Expressions;

namespace Util
{
    public static class ExtensionsToFastActivator
    {
        public static void FastInvoke<T>(this T target, Type[] genericTypes, Expression<Action<T>> expression,
                                         params object[] args)
        {
            FastInvoker<T>.Current.FastInvoke(target, genericTypes, expression, args);
        }
    }
}
