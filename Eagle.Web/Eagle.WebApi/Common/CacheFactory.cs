using Eagle.WebApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Caches;
using Util.Helpers;

namespace Eagle.WebApi.Common
{
    public class CacheFactory
    {
        private static ISectionCacheService _cacheService;

        static CacheFactory()
        {
            _cacheService = Ioc.Create<ISectionCacheService>();
        }

        /// <summary>
        /// 获取一个缓存对象
        /// </summary>
        /// <typeparam name="T">缓存对象的类型</typeparam>
        /// <param name="sectionType">缓存缓存section类型</param>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        public static TValue Get<TValue>(CacheSectionEnum sectionType, string key) where TValue : class
        {
            var sectionName = GetCacheSectionName(sectionType);
            return _cacheService.Get<TValue>(sectionName, key);
        }

        /// <summary>
        /// 从缓存中删除一个key
        /// </summary>
        /// <param name="sectionType">缓存缓存section类型</param>
        /// <param name="key">缓存key</param>
        public static void RemoveKey(CacheSectionEnum sectionType, string key)
        {
            var sectionName = GetCacheSectionName(sectionType);
            _cacheService.RemoveKey(sectionName, key);
        }

        /// <summary>
        /// 在缓存中存储一个kev/value对，缓存永久有效
        /// </summary>
        /// <param name="content">缓存对象</param>
        public static void Store<TValue>(TValue content) where TValue : ICacheStoredObject
        {
            var key = content.Key;
            var sectionName = GetCacheSectionName(content.CacheSectionType);
            _cacheService.Store(sectionName, key, content);
        }

        /// <summary>
        /// 在缓存中存储一个kev/value对，指定缓存失效时间
        /// </summary>
        /// <param name="content">缓存对象</param>
        /// <param name="durationMinute">缓存失效时间，单位：分钟</param>
        public static void Store<TValue>(TValue content, int durationMinute) where TValue : ICacheStoredObject
        {
            var key = content.Key;
            var sectionName = GetCacheSectionName(content.CacheSectionType);
            _cacheService.Store(sectionName, key, content, durationMinute);
        }


        private static string GetCacheSectionName(CacheSectionEnum sectionType)
        {
            switch (sectionType)
            {
                case CacheSectionEnum.Test:
                    return "Test";
                default:
                    return string.Empty;
            }
        }
    }
}
