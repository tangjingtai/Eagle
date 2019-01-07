using System;
using System.Threading.Tasks;
using Util.Events.Handlers;

namespace Util.Events {
    /// <summary>
    /// 事件总线
    /// </summary>
    public interface IEventBus {
        /// <summary>
        /// 发布事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">事件</param>
        Task PublishAsync<TEvent>( TEvent @event ) where TEvent : Event;

        /// <summary>
        /// 事件订阅，添加事件处理器
        /// </summary>
        /// <param name="host"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="handlerConfigurator"></param>
        /// <returns></returns>
        Task SubscribeAsync(string host, string userName = "guest", string password = "guest",
            Action<IEventHandlerConfiguration> handlerConfigurator = null);

        /// <summary>
        /// Stops the event bus.
        /// </summary>
        /// <returns>Task.</returns>
        Task StopEventBusAsync();
    }
}
