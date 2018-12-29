using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Util.Events;
using Util.Events.Handlers;
using Util.Events.Messages;
using Util.Helpers;

namespace Util.EventBus.RabbitMQ
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public class EventBus : IEventBus {
        
        /// <summary>
        /// 事件处理器服务
        /// </summary>
        public IEventHandlerManager HandlerManager { get; set; }

        /// <summary>
        /// 消息事件总线
        /// </summary>
        public IEventPublisher EventPublisher { get; set; }

        /// <summary>
        /// 初始化事件总线
        /// </summary>
        /// <param name="manager">事件处理器服务管理器</param>
        /// <param name="eventPublisher">消息发送器</param>
        public EventBus(IEventHandlerManager manager, IEventPublisher eventPublisher)
        {
            HandlerManager = manager;
            EventPublisher = eventPublisher;
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件</param>
        public async Task PublishAsync<TEvent>( TEvent @event ) where TEvent : Event {
            await EventPublisher.PublishAsync(@event);
        }

        /// <summary>
        /// 添加事件订阅
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="host"></param>
        /// <param name="queue"></param>
        /// <param name="concurrentConsumers"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task SubscribeAt<TEvent>(string host, string queue, int concurrentConsumers = 5,
            string userName = "guest",
            string password = "guest") where TEvent : Event
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                var mqHost = config.Host(new Uri($"rabbitmq://{host}:/"), h =>
                {
                    h.Username(userName);
                    h.Password(password);
                });
                var handlers = HandlerManager.GetHandlers<TEvent>();
                if (handlers == null || handlers.Count == 0)
                    return;
                var eventType = typeof(TEvent);
                foreach (var handler in handlers)
                {
                    var handlerType = handler.GetType();
                    config.ReceiveEndpoint(mqHost, queue, endpoint =>
                    {
                        endpoint.PrefetchCount = (ushort)concurrentConsumers;
                        endpoint.Durable = true;
                        endpoint.AutoDelete = false;
                        this.FastInvoke(new Type[] { eventType, handlerType }, x => x.SubscribeAt<Event, IEventHandler<Event>>(endpoint, handlerType));
                    });
                }
            });
            await bus.StartAsync();
        }

        /// <summary>
        /// 订阅消息，添加消息处理器
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="endpointConfiguration"></param>
        /// <param name="handlerType"></param>
        private void SubscribeAt<TEvent, THandler>(IRabbitMqReceiveEndpointConfigurator endpointConfiguration, Type handlerType) 
            where THandler : IEventHandler<TEvent>
            where TEvent : Event
        {
            endpointConfiguration.Handler<TEvent>(async context =>
            {
                try
                {
                    await Ioc.Create<THandler>().HandleAsync(context.Message);
                }
                catch (StoppedConsumeException)
                {
                    // 消费者停止消费消息后，将异常抛出，让消息回滚至原队列中
                    // 等待下次处理
                    throw;
                }
                catch (Exception ex)
                {
                    //Log.Write(string.Format("执行{0}错误", typeof(THandler)), MessageType.Error, this.GetType(), ex);
                }
            });
        }
    }
}
