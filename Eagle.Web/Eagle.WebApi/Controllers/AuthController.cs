using System;
using Eagle.WebApi.Common;
using Eagle.WebApi.Models.Auth.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eagle.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// 账号密码认证
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Authenticate([FromBody]AuthenticateRequest request)
        {
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(1);
            var token = JWTHelper.CreateToken(new JWTPayloadInfo { Subject = "123", Name = "唐景泰", Role = "admin", Email ="xxxx@qq.com" }, expiresAt);
            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                profile = new
                {
                    sid = "1",
                    name = "唐景泰",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
    }
}
