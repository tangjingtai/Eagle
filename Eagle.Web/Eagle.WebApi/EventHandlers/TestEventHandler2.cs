﻿using Eagle.WebApi.EventHandlers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Events.Handlers;

namespace Eagle.WebApi.EventHandlers
{
    public class TestEventHandler2 : IEventHandler<TestEvent2>
    {
        public Task HandleAsync(TestEvent2 @event)
        {
            Console.WriteLine($"TestEventHandler2 --  Number:{@event.Number}, Content:{@event.Content}");
            return Task.FromResult(0);
        }
    }
}
