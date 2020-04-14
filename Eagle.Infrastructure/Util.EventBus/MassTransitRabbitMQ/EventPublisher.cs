using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Util.Events;
using Util.Events.Handlers;

namespace Util.EventBus.RabbitMQ
{
    /// <summary>
    /// 事件发送器
    /// </summary>
    public class EventPublisher : IEventPublisher
    {
        /// <summary>
        /// _busControl
        /// </summary>
        IBusControl _busControl;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public EventPublisher(string host, string userName ="guest", string password = "guest")
        {
            //var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            //{
            //    var mqHost = config.Host(new Uri($"rabbitmq://{host}:/"), h =>
            //    {
            //        h.Username(userName);
            //        h.Password(password);
            //    });                
            //});
            //_busControl = bus;
            //bus.Start();
        }

        /// <summary>
        /// 发布消息事件
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event
        {
            await _busControl.Publish(@event);
        }
    }
}
