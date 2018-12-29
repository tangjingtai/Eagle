
using Microsoft.Extensions.DependencyInjection;
using System;
using Util.Caches.Couchbase;

namespace Util.Caches
{
    public static partial class Extensions
    {
        /// <summary>
        /// 使用couchbase作为缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">设置couchbase配置信息</param>
        /// <returns></returns>
        public static IServiceCollection UseCouchbaseCache(this IServiceCollection services, Action<CouchbaseConfig> action)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var config = new CouchbaseConfig();
            action.Invoke(config);
            if (config.Urls == null || config.Urls.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供url");
            if (config.BucketAndPassword == null || config.BucketAndPassword.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供bucket");

            services.AddSingleton<ISectionCacheService>(new CouchbaseCacheService(config));

            return services;
        }
    }
}
