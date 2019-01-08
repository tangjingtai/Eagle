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

using Eagle.Modules.DTO.CommonModule.Response;
using Eagle.Modules.ICommonModule.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eagle.Application.Service
{
    public interface ICommonModuleService
    {
        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        /// <returns>SystemConfig.</returns>
        SystemConfig GetSystemConfig();
    }

    internal class CommonModuleService : ICommonModuleService
    {
        /// <summary>
        /// The system configuration service
        /// </summary>
        readonly ISystemConfigService _systemConfigService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonModuleService"/> class.
        /// </summary>
        /// <param name="systemConfigService">The system configuration service.</param>
        public CommonModuleService(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }

        /// <summary>
        /// Gets the system configuration.
        /// </summary>
        /// <returns>SystemConfig.</returns>
        public SystemConfig GetSystemConfig()
        {
            return _systemConfigService.GetSystemConfig();
        }
    }
}
