using Eagle.WebApi.EventHandlers.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util.Events.Handlers;

namespace Eagle.WebApi.EventHandlers
{
    public class TestEventHandler : IEventHandler<TestEvent>
    {
        public Task HandleAsync(TestEvent @event)
        {
            throw new NotImplementedException();
        }
    }
}
