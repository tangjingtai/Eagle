using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Util.Domains;

namespace Eagle.Modules.CommonModule.DomainObject.Aggregate
{
    public class SystemConfigAggregate : AggregateRoot<SystemConfigAggregate, int>
    {
        /// <summary>
        /// 初始化SystemConfigAggregate
        /// </summary>
        public SystemConfigAggregate():base(0)
        {

        }

        /// <summary>
        /// 所有配置信息
        /// </summary>
        public List<SystemConfigEntity> SystemConfigs { get; set; }

        /// <summary>
        /// 根据配置名称获取配置的值
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public string GetConfigValue(string configName) => SystemConfigs?.FirstOrDefault(x => x.ConfigName.Equals(configName, StringComparison.OrdinalIgnoreCase))?.ConfigValue;
    }
}
