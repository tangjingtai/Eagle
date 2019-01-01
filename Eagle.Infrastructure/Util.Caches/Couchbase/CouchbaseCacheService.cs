using System.Linq;
using Couchbase;
using Couchbase.Authentication;
using Couchbase.Configuration.Client;
using ProtoBuf;
using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

namespace Util.Caches.Couchbase
{
    public class CouchbaseCacheService : ISectionCacheService, ICacheService
    {
        /// <summary>
        /// couchbase 工厂对象
        /// </summary>
        private Cluster _cluster;

        private string _defaultBucketName;

        /// <summary>
        /// 构造函数，不会对couchbase客户端进行初始化
        /// </summary>
        /// <param name="defaultBucketName"></param>
        public CouchbaseCacheService(string defaultBucketName = null)
        {
            _defaultBucketName = defaultBucketName;
        }
        /// <summary>
        /// 构造函数，对couchbase客户端进行初始化
        /// </summary>
        /// <param name="config"></param>
        /// <param name="defaultBucketName"></param>
        public CouchbaseCacheService(CouchbaseConfig config, string defaultBucketName = null)
        {
            _defaultBucketName = defaultBucketName;
            InitClusterClient(config);
        }

        /// <summary>
        /// 初始化couchbase客户端
        /// </summary>
        /// <param name="config"></param>
        public void InitClusterClient(CouchbaseConfig config)
        {
            config.CheckNull(nameof(config));
            if (config.Urls == null || config.Urls.Count == 0)
                throw new ArgumentNullException(nameof(config), "couchbase 配置错误，必须提供url");
            if (config.BucketAndPassword == null || config.BucketAndPassword.Count == 0)
                throw new ArgumentNullException(nameof(config), "couchbase 配置错误，必须提供bucket");
            if(_cluster != null)
                _cluster.Dispose();
            if (config != null && config.Urls != null && config.Urls.Count > 0)
            {
                var clientConfiguration = new ClientConfiguration
                {
                    Servers = config.Urls.Select(x => new Uri(x)).ToList()
                };
                _cluster = new Cluster(clientConfiguration);
                foreach (var kv in config.BucketAndPassword ?? new Dictionary<string, string>())
                {
                    var authenticator = new PasswordAuthenticator(kv.Key, kv.Value);
                    _cluster.Authenticate(authenticator);
                }
            }
        }

        public TValue Get<TValue>(string sectionName, string key) where TValue : class
        {
            using (var bucket = _cluster.OpenBucket(sectionName))
            {
                var result = bucket.Get<byte[]>(key);
                if (result != null && result.Success && result.Value != null)
                {
                    return Deserialize<TValue>(result.Value);
                }
                return default(TValue);
            }
        }

        public void RemoveKey(string sectionName, string key)
        {
            using (var bucket = _cluster.OpenBucket(sectionName))
            {
                bucket.Remove(key);
            }
        }

        public void Store(string sectionName, string key, object content)
        {
            using (var bucket = _cluster.OpenBucket(sectionName))
            {
                bucket.Upsert(key, GetBytes(content));
            }
        }

        public void Store(string sectionName, string key, object content, int durationMinute)
        {
            if (durationMinute <= 0)
            {
                Store(sectionName, key, content);
                return;
            }
            using (var bucket = _cluster.OpenBucket(sectionName))
            {
                var timeSpan = new TimeSpan(0, (int)durationMinute, 0);
                bucket.Upsert(key, GetBytes(content));
            }
        }


        #region ICacheService的实现

        TValue ICacheService.Get<TValue>(string key)
        {
            return Get<TValue>(_defaultBucketName, key);
        }

        void ICacheService.RemoveKey(string key)
        {
            RemoveKey(_defaultBucketName, key);
        }

        void ICacheService.Store(string key, object content)
        {
            using (var bucket = _cluster.OpenBucket(_defaultBucketName))
            {
                bucket.Upsert(key, GetBytes(content));
            }
        }

        void ICacheService.Store(string key, object content, int durationMinute)
        {
            using (var bucket = _cluster.OpenBucket(_defaultBucketName))
            {
                var timeSpan = new TimeSpan(0, (int)durationMinute, 0);
                bucket.Upsert(key, GetBytes(content), timeSpan);
            }
        }

        #endregion

        #region 使用protobuf对缓存内容编码、解码

        /// <summary>
        ///  获取object的byte
        /// </summary>
        /// <param name="obje"></param>
        /// <returns></returns>
        private byte[] GetBytes(object obje)
        {
            if (obje == null)
                return null;

            byte[] bytes = null;
            using (var stream = new MemoryStream())
            {
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, obje);
                    ms.Position = 0;
                    using (GZipStream compressionStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        ms.CopyTo(compressionStream);
                    }
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }

        /// <summary>
        /// 对象反序列化
        /// </summary>
        /// <typeparam name="T">反序列化后对象类型</typeparam>
        /// <param name="bytes">待反序列化对象的byte数组</param>
        /// <returns>反序列化后对象</returns>
        private T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null)
                return default(T);

            using (var stream = ToStream(bytes))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// 将bytes写入到MemoryStream中
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private Stream ToStream(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Flush();
                ms.Position = 0;
                MemoryStream stream = new MemoryStream();
                using (GZipStream decompressStream = new GZipStream(ms, CompressionMode.Decompress))
                {
                    decompressStream.Flush();
                    decompressStream.CopyTo(stream);
                    stream.Position = 0;
                    return stream;
                }

            }
        }

        #endregion
    }
}