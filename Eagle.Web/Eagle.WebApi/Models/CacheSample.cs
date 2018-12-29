using Eagle.WebApi.Common;
using Eagle.WebApi.Models.Enums;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eagle.WebApi.Models
{
    [ProtoContract]
    public class CacheSample : ICacheStoredObject
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Desc { get; set; }

        [ProtoMember(3)]
        public List<int> Set { get; set; }

        public string Key => $"CacheSample_{Id}";

        public CacheSectionEnum CacheSectionType =>  CacheSectionEnum.Test;
    }
}
