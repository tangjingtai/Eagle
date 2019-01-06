
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Util.Domains;

namespace Eagle.Modules.CommonModule.DomainObject
{
    /// <summary>
    /// 初始化SystemConfigEntity
    /// </summary>
    public class SystemConfigEntity : AggregateRoot<SystemConfigEntity, string>
    {
        /// <summary>
        /// 初始化SystemConfigEntity对象
        /// </summary>
        /// <param name="configName"></param>
        public SystemConfigEntity(string configName)
            :base(configName)
        {
            ConfigName = configName;
        }

        /// <summary>
        /// 配置名称
        /// </summary>
        [DisplayName("配置名称")]
        [Required(ErrorMessage = "配置名称不能为空")]
        [StringLength(45, ErrorMessage = "配置名称输入过长，不能超过45位")]
        public string ConfigName { get; set; }

        /// <summary>
        /// 配置值
        /// </summary>
        [DisplayName("配置值")]
        [Required(ErrorMessage = "配置值不能为空")]
        [StringLength(600, ErrorMessage = "配置值输入过长，不能超过45位")]
        public string ConfigValue { get; set; }

        /// <summary>
        /// 配置描述
        /// </summary>
        [DisplayName("配置描述")]
        public string Description { get; set; }
    }
}
