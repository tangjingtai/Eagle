// ***********************************************************************
// Assembly         : Eagle.Application.Service
// Author           : 唐景泰
// Created          : 01-08-2019
//
// Last Modified By : 唐景泰
// Last Modified On : 01-08-2019
// ***********************************************************************
//  开源版本，随意使用，绝不追究版权问题
//
// <summary></summary>
// ***********************************************************************

using Eagle.Application.Service;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 应用服务的扩展类
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 添加模块的服务配置
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddModuleService(this IServiceCollection services)
        {
            services.AddCommonModule();

            services.AddScoped<ICommonModuleService, CommonModuleService>();

            return services;
        }
    }
}
