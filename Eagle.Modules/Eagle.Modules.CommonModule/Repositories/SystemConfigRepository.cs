// ***********************************************************************
// Assembly         : Eagle.Modules.CommonModule
// Author           : 唐景泰
// Created          : 01-04-2019
//
// Last Modified By : 唐景泰
// Last Modified On : 01-05-2019
// ***********************************************************************
//  开源版本，随意使用，绝不追究版权问题
//
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Eagle.Modules.CommonModule.DomainObject;
using Util.Datas.Sql.Queries;

namespace Eagle.Modules.CommonModule.Repositories
{
    /// <summary>
    /// Class SystemConfigRepository.
    /// </summary>
    /// <seealso cref="Eagle.Modules.CommonModule.Repositories.ISystemConfigRepository" />
    internal class SystemConfigRepository : ISystemConfigRepository
    {
        /// <summary>
        /// The SQL query
        /// </summary>
        readonly ISqlQuery _sqlQuery;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigRepository" /> class.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        public SystemConfigRepository(ISqlQuery sqlQuery)
        {
            _sqlQuery = sqlQuery;
        }

        /// <summary>
        /// Finds all.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>List&lt;SystemConfigEntity&gt;.</returns>
        public List<SystemConfigEntity> FindAll(Expression<Func<SystemConfigEntity, bool>> predicate = null)
        {
            return _sqlQuery.Select<SystemConfigEntity>(x => new object[] { x.ConfigName, x.ConfigValue, x.Description })
                 .From<SystemConfigEntity>()
                 .Where<SystemConfigEntity>(predicate)
                 .ToList<SystemConfigEntity>();
        }

        /// <summary>
        /// Finds all asynchronous.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>System.Threading.Tasks.Task&lt;System.Collections.Generic.List&lt;Eagle.Modules.CommonModule.DomainObject.SystemConfigEntity&gt;&gt;.</returns>
        public async Task<List<SystemConfigEntity>> FindAllAsync(Expression<Func<SystemConfigEntity, bool>> predicate = null)
        {
            return await _sqlQuery.Select<SystemConfigEntity>(x => new object[] { x.ConfigName, x.ConfigValue, x.Description })
                 .From<SystemConfigEntity>()
                 .Where<SystemConfigEntity>(predicate)
                 .ToListAsync<SystemConfigEntity>();
        }
    }
}
