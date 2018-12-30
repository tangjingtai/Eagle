

using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Eagle.WebApi.Common
{
    /// <summary>
    /// 常量配置
    /// </summary>
    public class ConstantFactory
    {
        /// <summary>
        /// 对称加密秘钥
        /// </summary>
        public static readonly SymmetricSecurityKey SymmetricKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("eagle flying in the sky."));

        /// <summary>
        /// Valid Audience
        /// </summary>
        public const string ValidAudience = "api";

        /// <summary>
        /// Valid Issuer
        /// </summary>
        public const string ValidIssuer = "Eagle.API";
    }
}
