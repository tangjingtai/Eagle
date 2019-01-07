
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
        /// <para>注意：使用该方法时，还需要在服务注册完成之后，调用<see cref="CouchbaseExtensions.InitCouchBaseCacheClusterClient(IServiceProvider, Action{CouchbaseConfig})"/></para>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurator">设置couchbase配置信息</param>
        /// <returns></returns>
        public static IServiceCollection AddCouchBaseCache(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddSingleton<ISectionCacheService>(new CouchbaseCacheService());
            return services;
        }

        /// <summary>
        /// 使用couchbase作为缓存，并对couchbase客户端进行初始化
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configurator">设置couchbase配置信息</param>
        /// <returns></returns>
        public static IServiceCollection AddCouchBaseCache(this IServiceCollection services, Action<CouchbaseConfig> configurator)
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

        /// <summary>
        /// 初始化couchbase客户端
        /// </summary>
        /// <param name="serviceProvide"></param>
        /// <param name="configurator"></param>
        public static void InitCouchBaseCacheClusterClient(IServiceProvider serviceProvide, Action<CouchbaseConfig> configurator)
        {
            if (serviceProvide == null)
                throw new ArgumentNullException(nameof(serviceProvide));
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));

            var config = new CouchbaseConfig();
            configurator.Invoke(config);
            if (config.Urls == null || config.Urls.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供url");
            if (config.BucketAndPassword == null || config.BucketAndPassword.Count == 0)
                throw new Exception("couchbase 配置错误，必须提供bucket");

            var cacheService = serviceProvide.GetService<ICacheService>();
            if (cacheService is CouchbaseCacheService couchbaseCacheService)
                couchbaseCacheService.InitClusterClient(config);
        }
    }
}
