// ***********************************************************************
// Assembly         : Eagle.Modules.CommonModule
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

using Eagle.Modules.CommonModule.Repositories;
using Eagle.Modules.CommonModule.Services;
using Eagle.Modules.ICommonModule.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 公共模块的扩展类型
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// 添加公共模块的服务配置
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddCommonModule(this IServiceCollection services)
        {
            services.AddScoped<ISystemConfigRepository, SystemConfigRepository>();
            services.AddScoped<ISystemConfigService, SystemConfigService>();
            return services;
        }
    }
}
