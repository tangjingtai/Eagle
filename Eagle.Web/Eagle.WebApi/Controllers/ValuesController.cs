using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI.Model.Recommend;
using Eagle.WebApi.Common;
using Eagle.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Util.Caches;

namespace Eagle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        ISectionCacheService _cacheService;

        public ValuesController(ISectionCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var key = $"CacheSample_{id}";
            var sample = new CacheSample
            {
                Id = id,
                Desc = $"desc {id}",
                Set = new List<int> { 1, 2, 3 }
            };
            CacheFactory.Store(sample, 10);

            var sample2 = CacheFactory.Get<CacheSample>(Models.Enums.CacheSectionEnum.Test, key);

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
