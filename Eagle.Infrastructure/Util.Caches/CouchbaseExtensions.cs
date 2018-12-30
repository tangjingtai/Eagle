
using Microsoft.Extensions.DependencyInjection;
using System;
using Util.Caches;
using Util.Caches.Couchbase;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CouchbaseExtensions
    {
        /// <summary>
        /// 使用couchbase作为缓存
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurator">设置couchbase配置信息</param>
        /// <returns></returns>
        public static IServiceCollection AddCouchbaseCache(this IServiceCollection services, Action<CouchbaseConfig> configurator)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var config = new CouchbaseConfig();
            configurator.Invoke(config);
            if (config.Urls == null || config.Urls.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供url");
            if (config.BucketAndPassword == null || config.BucketAndPassword.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供bucket");

            services.AddSingleton<ISectionCacheService>(new CouchbaseCacheService(config));

            return services;
        }
    }
}
