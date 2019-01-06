// ***********************************************************************
// Assembly         : Eagle.Modules.ICommonModule
// Author           : 唐景泰
// Created          : 01-06-2019
//
// Last Modified By : 唐景泰
// Last Modified On : 01-06-2019
// ***********************************************************************
//  开源版本，随意使用，绝不追究版权问题
//
// <summary></summary>
// ***********************************************************************

using Eagle.Modules.DTO.CommonModule.Response;

namespace Eagle.Modules.ICommonModule.Services
{
    /// <summary>
    /// 提供系统配置相关操作
    /// </summary>
    public interface ISystemConfigService
    {
        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        /// <returns>SystemConfig.</returns>
        SystemConfig GetSystemConfig();

        /// <summary>
        /// Gets the name of the system configuration by.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <returns>System.Object.</returns>
        object GetSystemConfigByName(string configName);
    }
}
