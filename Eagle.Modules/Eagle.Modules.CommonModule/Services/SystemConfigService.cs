// ***********************************************************************
// Assembly         : Eagle.Modules.CommonModule
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

using Eagle.Modules.CommonModule.DomainObject;
using Eagle.Modules.CommonModule.Repositories;
using Eagle.Modules.DTO.CommonModule.Response;
using Eagle.Modules.ICommonModule.Services;
using System.Linq;
using Util;

namespace Eagle.Modules.CommonModule.Services
{
    /// <summary>
    /// Class SystemConfigService.
    /// </summary>
    /// <seealso cref="Eagle.Modules.ICommonModule.Services.ISystemConfigService" />
    internal class SystemConfigService : ISystemConfigService
    {
        /// <summary>
        /// The system configuration repository
        /// </summary>
        ISystemConfigRepository _systemConfigRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigService"/> class.
        /// </summary>
        /// <param name="systemConfigRepository">The system configuration repository.</param>
        public SystemConfigService(ISystemConfigRepository systemConfigRepository)
        {
            _systemConfigRepository = systemConfigRepository;
        }

        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        /// <returns>SystemConfig.</returns>
        public SystemConfig GetSystemConfig()
        {
            var allConfigurations = _systemConfigRepository.FindAll();
            return allConfigurations.BuildObjectFromRow<SystemConfig, SystemConfigEntity>("ConfigName", "ConfigValue");
        }

        /// <summary>
        /// Gets the name of the system configuration by.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <returns>System.Object.</returns>
        public object GetSystemConfigByName(string configName)
        {
            var allConfiguration = _systemConfigRepository.FindAll(x=>x.ConfigName == configName);
            return allConfiguration?.FirstOrDefault()?.ConfigValue;
        }
    }
}
