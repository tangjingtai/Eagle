namespace Util.Caches
{
    public interface ICacheService
    {
        /// <summary>
        /// 获取一个缓存对象
        /// </summary>
        /// <typeparam name="T">缓存对象的类型</typeparam>
        /// <param name="key">缓存key</param>
        /// <returns></returns>
        TValue Get<TValue>(string key) where TValue : class;

        /// <summary>
        /// 从缓存中删除一个key
        /// </summary>
        /// <param name="key"></param>
        void RemoveKey(string key);

        /// <summary>
        /// 在缓存中存储一个kev/value对，缓存永久有效
        /// </summary>
        /// <param name="key">缓存中的key</param>
        /// <param name="content">缓存对象</param>
        void Store(string key, object content);

        /// <summary>
        /// 在缓存中存储一个kev/value对，指定缓存失效时间
        /// </summary>
        /// <param name="key">缓存中的key</param>
        /// <param name="content">缓存对象</param>
        /// <param name="durationMinute">缓存失效时间，单位：分钟</param>
        void Store(string key, object content, int durationMinute);
    }
}