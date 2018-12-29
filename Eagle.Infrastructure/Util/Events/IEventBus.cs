using System.Threading.Tasks;

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
        /// 添加事件订阅
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="host"></param>
        /// <param name="queue"></param>
        /// <param name="concurrentConsumers"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task SubscribeAt<TEvent>(string host, string queue, int concurrentConsumers = 5,
            string userName = "guest",
            string password = "guest") where TEvent : Event;
    }
}
