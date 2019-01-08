using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eagle.WebApi.Common;
using Eagle.WebApi.Models.Enums;
using ProtoBuf;

namespace AI.Model.Recommend
{
    /// <summary>
    /// 推荐内容实体
    /// </summary>
    [ProtoContract]
    public class RecommendItem : ICacheStoredObject
    {
        /// <summary>
        /// 缓存key的格式
        /// </summary>
        private const string _keyFormat = "RecommendItem_{0}_{1}";

        /// <summary>
        /// 内容Id，同一个内容平台中必须唯一
        /// </summary>
        [ProtoMember(1)]
        public string ItemId { get; set; }

        /// <summary>
        /// 内容平台，见枚举：AI.Model.Enum.RecommendItemPlatformEnum
        /// </summary>
        [ProtoMember(2)]
        public int ItemPlatform { get; set; }

        /// <summary>
        /// 内容的类型Id，由内容平台自定义
        /// </summary>
        [ProtoMember(3)]
        public int ItemTypeId { get; set; }

        /// <summary>
        /// 推荐内容的标题
        /// </summary>
        [ProtoMember(4)]
        public string Title { get; set; }

        /// <summary>
        /// 推荐内容的标签集合
        /// </summary>
        [ProtoMember(5)]
        public string[] Tags { get; set; }

        /// <summary>
        /// 推荐内容的描述信息1；
        /// 万门大学视频课程：课程简介
        /// </summary>
        [ProtoMember(6)]
        public string ItemDesc1 { get; set; }

        /// <summary>
        /// 推荐内容的描述信息2；
        /// 万门大学视频课程：讲师介绍
        /// </summary>
        [ProtoMember(7)]
        public string ItemDesc2 { get; set; }

        /// <summary>
        /// 推荐内容的描述信息3
        /// </summary>
        [ProtoMember(8)]
        public string ItemDesc3 { get; set; }

        /// <summary>
        /// 推荐内容的描述信息4
        /// </summary>
        [ProtoMember(9)]
        public string ItemDesc4 { get; set; }

        /// <summary>
        /// 推荐内容的描述信息5
        /// </summary>
        [ProtoMember(10)]
        public string ItemDesc5 { get; set; }
        
        #region ICouchBaseStoredObject实现

        /// <summary>
        /// couchbase中的bucket类型
        /// </summary>
        public CacheSectionEnum CacheSectionType
        {
            get
            {
                return CacheSectionEnum.Test;
            }
        }

        /// <summary>
        /// 缓存key
        /// </summary>
        public string Key
        {
            get
            {
                return GetCacheKey(ItemId, ItemPlatform);
            }
        }

        #endregion

        /// <summary>
        /// 获取缓存Key
        /// </summary>
        /// <param name="itemId">推荐内容Id</param>
        /// <param name="itemPlatformType">推荐内容平台类型</param>
        /// <returns></returns>
        public static string GetCacheKey(string itemId, int itemPlatformType)
        {
            return string.Format(_keyFormat, itemId, itemPlatformType);
        }
    }
}
