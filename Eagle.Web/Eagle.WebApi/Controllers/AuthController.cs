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
            var tokenHandler = new JwtSecurityTokenHandler();
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddDays(1);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(JwtClaimTypes.Audience, ConstantFactory.ValidAudience),
                    new Claim(JwtClaimTypes.Issuer,ConstantFactory.ValidIssuer),
                    new Claim(JwtClaimTypes.Id, "1"),
                    new Claim(JwtClaimTypes.Name, "xxx"),
                    new Claim(JwtClaimTypes.Email, "xxx@qq.com")
                }),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(ConstantFactory.SymmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return Ok(new
            {
                access_token = tokenString,
                token_type = "Bearer",
                profile = new
                {
                    sid = "1",
                    name = "xxxx",
                    auth_time = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                    expires_at = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
                }
            });
        }
    }
}
