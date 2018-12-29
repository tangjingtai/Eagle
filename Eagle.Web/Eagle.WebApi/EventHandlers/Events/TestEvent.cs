using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Events;

namespace Eagle.WebApi.EventHandlers.Events
{
    public class TestEvent : IEvent
    {
        private string _id = Guid.NewGuid().ToString("N");
        public string Id { get => _id; set => _id = value; }

        public DateTime Time => DateTime.Now;
    }
}
