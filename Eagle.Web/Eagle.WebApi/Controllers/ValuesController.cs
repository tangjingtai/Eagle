using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI.Model.Recommend;
using Eagle.WebApi.Common;
using Eagle.WebApi.EventHandlers.Events;
using Eagle.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Util.Caches;
using Util.Events;
using Util.Events.Handlers;
using Util.Helpers;

namespace Eagle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        ISectionCacheService _cacheService;

        ILogger<ValuesController> _logger;

        public ValuesController(ISectionCacheService cacheService, ILogger<ValuesController> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        [Authorize(Roles = "guest,admin", Policy = "Permission")]
        public ActionResult<IEnumerable<string>> Get()
        {
            _logger.LogInformation("values get");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [Authorize(Roles = "admin", Policy = "Permission")]
        public async Task<ActionResult<string>> Get(int id)
        {
            if (id == 1)
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
            }
            if (id == 2)
            {
                for (var i = 0; i < 1; i++)
                {
                    var publisher = Ioc.Create<IEventBus>();
                    await publisher.PublishAsync(new TestEvent { Number = 100, Content = "测试内容" });
                    await publisher.PublishAsync(new TestEvent2 { Number = 100, Content = "测试内容" });
                }
            }
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
