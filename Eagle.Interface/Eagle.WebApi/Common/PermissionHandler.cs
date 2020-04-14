using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

namespace Eagle.WebApi.Common
{
    /// <summary>
    /// 权限授权Handler
    /// </summary>
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        /// <summary>
        /// 验证方案提供对象
        /// </summary>
        public IAuthenticationSchemeProvider Schemes { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="schemes"></param>
        public PermissionHandler(IAuthenticationSchemeProvider schemes)
        {
            Schemes = schemes;
        }

        /// <summary>
        /// handle requirement as an asynchronous operation.
        /// <para>
        /// 验证是否拥有requirement中所指定的授权
        /// </para>
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        /// <returns>Task.</returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //从AuthorizationHandlerContext转成HttpContext，以便取出表求信息
            var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext;
            //判断请求是否停止
            var handlers = httpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            foreach (var scheme in await Schemes.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null && await handler.HandleRequestAsync())
                {
                    context.Fail();
                    return;
                }
            }
            
           //判断请求是否拥有凭据，即有没有登录
           var defaultAuthenticate = await Schemes.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                //result?.Principal不为空即表示已登录
                if (result?.Principal != null && result.Principal.Claims != null && result.Principal.Claims.Count() >0)
                {
                    var subjectClaim = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                    if(subjectClaim == null)
                    {
                        context.Fail();
                        return;
                    }
                    var userId = subjectClaim.Value;
                    // TODO: 对用户其他方面的操作
                    httpContext.User = result.Principal;
                    context.Succeed(requirement);
                }
                else
                    context.Fail();
                return;
            }
            context.Succeed(requirement);
        }
    }

    /// <summary>
    /// PermissionRequirement
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Audience(s) that this ID Token is intended for. It MUST contain the OAuth 2.0
        ///     client_id of the Relying Party as an audience value. It MAY also contain identifiers
        ///     for other audiences. In the general case, the aud value is an array of case sensitive
        ///     strings. In the common special case when there is one audience, the aud value
        ///     MAY be a single case sensitive string.
        /// </summary>
        public string Audience { get; set; } = JWTHelper.AUDIENCE;

        /// <summary>
        /// Issuer Identifier for the Issuer of the response. The iss value is a case sensitive
        ///     URL using the https scheme that contains scheme, host, and optionally, port number
        ///     and path components and no query or fragment components.
        /// </summary>
        public string Issuer { get; set; } = JWTHelper.ISSUER;
    }
}
