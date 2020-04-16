using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Util.Helpers;
using System.Linq;
using Util.Webs.Clients;
using System.Threading.Tasks;

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
            var url = httpAttribute.Template;
            var webClient = new WebClient();
            IHttpRequest httpRequest = null;
            switch (httpAttribute.HttpMethods.First().ToUpper())
            {
                case "POST":
                    httpRequest = webClient.Post(url);
                    break;
                case "PUT":
                    httpRequest = webClient.Put(url);
                    break;
                case "GET":
                    httpRequest = webClient.Get(url);
                    break;
                case "DELETE":
                    httpRequest = webClient.Delete(url);
                    break;
                default:
                    return null;
            }
            for (var i = 0; i < parameters.Length; i++)
            {
                httpRequest.Data(parameters[i].Name, args[i]);
            }

            var reponse = httpRequest.ResultAsync().Result;
            if (targetMethod.ReturnType == typeof(void))
            {
                return null;
            }
            return Json.ToObject(reponse, targetMethod.ReturnType);
        }

        /// <summary>
        /// 构建Http请求的消息体
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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
                return Json.ToJson(args[0]);
            }
            else
            {
                var map = new Dictionary<string, object>();
                for (var i = 0; i < parameters.Length; i++)
                {
                    map.Add(parameters[0].Name, args[i]);
                }
                return Json.ToJson(map);
            }
        }

        /// <summary>
        /// 构建url传参，仅支持C#的基础类型传参
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string BuildUrlParameter(ParameterInfo[] parameters, object[] args)
        {
            var items = new List<string>();
            for(var i=0;i< parameters.Length;i++)
            {
                if(parameters[0].ParameterType.IsPrimitive)
                {
                    items.Add($"{parameters[0].Name}={args[0].ToString()}");
                }
            }
            return string.Join("&", items);
        }
    }
}
