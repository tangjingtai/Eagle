using System;
using System.Collections.Generic;
using System.Text;
using Util.Events;
using Util.Events.Handlers;
using Util.Helpers;

namespace Util.EventBus.RabbitMQ
{
    public class EventHandlerManager : IEventHandlerManager
    {
        public List<IEventHandler<TEvent>> GetHandlers<TEvent>() where TEvent : IEvent
        {
            return Ioc.CreateList<IEventHandler<TEvent>>();
        }
    }
}
