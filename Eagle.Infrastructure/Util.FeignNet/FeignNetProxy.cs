using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Util.FeignNet
{
    public class FeignNetProxy : DispatchProxy
    {
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            var httpAttribute = targetMethod.GetCustomAttribute<HttpMethodAttribute>();
            if (httpAttribute == null)
                return null;
            
            var parameters = targetMethod.GetParameters();

            return null;
        }

        private string BuildRequestBody(ParameterInfo[] parameters, object[] args)
        {
            if (parameters.Length == 0)
            {
                return "{}";
            }
            else if (parameters.Length == 1 && !parameters[0].ParameterType.IsPrimitive)
            {
                if(args[0] == null)
                    return "{}";
                return "";
            }
            else
            {
                // todo: 外包一层json
            }
            return null;
        }
    }
}
