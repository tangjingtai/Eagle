using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Validations;

namespace Eagle.WebApi.Common
{
    /// <summary>
    /// JWT操作帮助类
    /// </summary>
    public class JWTHelper
    {
        /// <summary>
        /// 对称加密秘钥
        /// </summary>
        public static readonly SymmetricSecurityKey SYMMTRIC_KEY = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("eagle flying in the sky."));

        /// <summary>
        /// Audience: api
        /// </summary>
        public const string AUDIENCE = JWTPayloadInfo.AUDIENCE;

        /// <summary>
        /// Issuer: Eagle.API
        /// </summary>
        public const string ISSUER = JWTPayloadInfo.ISSUER;

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="payload">payload信息</param>
        /// <param name="expiresAtUtc">指定过期的UTC时间</param>
        /// <returns></returns>
        public static string CreateToken(JWTPayloadInfo payload, DateTime expiresAtUtc)
        {
            payload.CheckNull(nameof(payload));
            if (!payload.Validate().IsValid)
                throw new ArgumentException(payload.Validate().First().ErrorMessage, nameof(payload));

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Issuer, payload.Issuer),
                new Claim(JwtClaimTypes.Audience, payload.Audience),
                new Claim(JwtClaimTypes.Subject, payload.Subject)
            };
            if (!string.IsNullOrEmpty(payload.Name))
                claims.Add(new Claim(JwtClaimTypes.Name, payload.Name));
            if (!string.IsNullOrEmpty(payload.Email))
                claims.Add(new Claim(JwtClaimTypes.Email, payload.Email));
            if (!string.IsNullOrEmpty(payload.Role))
                claims.Add(new Claim(JwtClaimTypes.Role, payload.Role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAtUtc,
                SigningCredentials = new SigningCredentials(SYMMTRIC_KEY, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }

        /// <summary>
        /// 从token中读取出payload的信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static JWTPayloadInfo ReadPayload(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var payloadInfo = new JWTPayloadInfo();
            var tokenObj = tokenHandler.ReadJwtToken(token);
            if (tokenObj.Claims == null || !tokenObj.Claims.Any())
                return payloadInfo;

            payloadInfo.Subject = tokenObj.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Subject, StringComparison.OrdinalIgnoreCase))?.Value;
            payloadInfo.Name = tokenObj.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Name, StringComparison.OrdinalIgnoreCase))?.Value;
            payloadInfo.Email = tokenObj.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Email, StringComparison.OrdinalIgnoreCase))?.Value;
            payloadInfo.Role = tokenObj.Claims.FirstOrDefault(x => x.Type.Equals(JwtClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value;
            return payloadInfo;
        }
    }

    /// <summary>
    /// JWT payload部分
    /// </summary>
    public class JWTPayloadInfo : IValidation
    {
        /// <summary>
        /// Audience
        /// </summary>
        internal const string AUDIENCE = "api";

        /// <summary>
        /// Issuer
        /// </summary>
        internal const string ISSUER = "Eagle.API";

        /// <summary>
        /// Unique Identifier for the End-User at the Issuer.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// End-User's full name in displayable form including all name parts, possibly including
        /// titles and suffixes, ordered according to the End-User's locale and preferences.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The role
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322]
        /// addr-spec syntax. The relying party MUST NOT rely upon this value being unique
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Audience(s) that this ID Token is intended for. It MUST contain the OAuth 2.0
        ///     client_id of the Relying Party as an audience value. It MAY also contain identifiers
        ///     for other audiences. In the general case, the aud value is an array of case sensitive
        ///     strings. In the common special case when there is one audience, the aud value
        ///     MAY be a single case sensitive string.
        /// </summary>
        public string Audience { get; set; } = AUDIENCE;

        /// <summary>
        /// Issuer Identifier for the Issuer of the response. The iss value is a case sensitive
        ///     URL using the https scheme that contains scheme, host, and optionally, port number
        ///     and path components and no query or fragment components.
        /// </summary>
        public string Issuer { get; set; } = ISSUER;

        public ValidationResultCollection Validate()
        {
            if (string.IsNullOrEmpty(Subject))
                return new ValidationResultCollection("Subject can not be null or empty.");

            return new ValidationResultCollection();            
        }
    }
}
