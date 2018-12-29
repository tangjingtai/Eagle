using System.Collections.Generic;

namespace Util.Caches.Couchbase
{
    /// <summary>
    /// couchbase 配置
    /// </summary>
    public class CouchbaseConfig
    {
        /// <summary>
        /// couchbase集群地址集合
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// couchbase中bucket名称和对应的密码
        /// </summary>
        public Dictionary<string, string> BucketAndPassword { get; set; }
    }
}
