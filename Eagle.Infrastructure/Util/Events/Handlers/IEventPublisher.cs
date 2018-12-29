
using System.Threading.Tasks;

namespace Util.Events.Handlers
{
    /// <summary>
    /// 事件发送器
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// 发布消息事件
        /// </summary>
        /// <typeparam name="TEvent">事件类型</typeparam>
        /// <param name="event">消息事件</param>
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : Event;
    }
}
