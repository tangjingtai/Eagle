using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Events;

namespace Eagle.WebApi.EventHandlers.Events
{
    public class TestEvent : Event, IEvent
    {
        public int Number { get; set; }

        public string Content { get; set; }
    }
}
