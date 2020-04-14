using Eagle.WebApi.EventHandlers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Util.Events.Handlers;

namespace Eagle.WebApi.EventHandlers
{
    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event)
        {
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [{Thread.CurrentThread.ManagedThreadId}]TestEventHandler -- Number:{@event.Number}, Content:{@event.Content}");
            Thread.Sleep(1000);
            return Task.FromResult(0);
        }
    }
}
