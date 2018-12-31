using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Eagle.WebApi.Common;
using Eagle.WebApi.Models.Auth.Requests;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Eagle.WebApi.Controllers
{
    [Route("api/[controller]")]
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
            var token = JWTHelper.CreateToken(new JWTPayloadInfo { Subject = "用户Id加密", Name = "用户真实姓名", Role = "用户角色" }, expiresAt);
            return Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                profile = new
                {
                    sid = "1",
                    name = "用户真实姓名",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
    }
}
