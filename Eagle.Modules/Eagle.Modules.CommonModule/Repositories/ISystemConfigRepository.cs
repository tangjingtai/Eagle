using Eagle.Modules.CommonModule.DomainObject;
using Util.Datas.Stores.Operations;

namespace Eagle.Modules.CommonModule.Repositories
{
    /// <summary>
    /// 系统配置项仓储接口
    /// </summary>
    internal interface ISystemConfigRepository 
        : IFindAll<SystemConfigEntity, string>, IFindAllAsync<SystemConfigEntity, string>
    {
    }
}
