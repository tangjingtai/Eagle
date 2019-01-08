using Eagle.WebApi.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eagle.WebApi.Common
{
    public interface ICacheStoredObject
    {
        /// <summary>
        ///  在缓存中的Key
        /// </summary>
        string Key { get; }

        /// <summary>
        ///  将存储到哪个缓存Section
        /// </summary>
        CacheSectionEnum CacheSectionType { get; }
    }
}
