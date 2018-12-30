
namespace Eagle.WebApi.Models.Auth.Requests
{
    /// <summary>
    /// 账号密码认证请求
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }
    }
}
