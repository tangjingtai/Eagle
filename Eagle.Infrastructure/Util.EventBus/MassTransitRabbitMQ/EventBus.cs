using MassTransit;
using System;
using System.Threading.Tasks;
using Util.Events;
using Util.Events.Handlers;

namespace Util.EventBus.MassTransitRabbitMQ
{
    /// <summary>
    /// 事件总线
    /// </summary>
    public class EventBus : IEventBus
    {
        /// <summary>
        /// 事件总线控制器
        /// </summary>
        IBusControl _busControl;

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件</param>
        public async Task PublishAsync<TEvent>( TEvent @event ) where TEvent : Event {
            if(_busControl == null)
            {
                throw new Exception("未配置消息发送器");
            }
            await _busControl.Publish(@event);
        }

        /// <summary>
        /// 事件订阅，添加事件处理器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="handlerConfigurator"></param>
        /// <returns></returns>
        public async Task SubscribeAsync(string host, string userName = "guest", string password = "guest", 
            Action<IEventHandlerConfiguration> handlerConfigurator = null)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(config =>
            {
                var mqHost = config.Host(new Uri($"rabbitmq://{host}:/"), h =>
                {
                    h.Username(userName);
                    h.Password(password);
                });
                IEventHandlerConfiguration handlerConfiguration = new EventHandlerConfiguration(config, mqHost);
                handlerConfigurator?.Invoke(handlerConfiguration);

            });
            _busControl = bus;
            await bus.StartAsync();
        }

        /// <summary>
        /// stop event bus as an asynchronous operation.
        /// </summary>
        /// <returns>Task.</returns>
        public async Task StopEventBusAsync()
        {
            if (_busControl == null)
                await Task.FromResult(0);
            else
                await _busControl.StopAsync();
        }

    }
}
