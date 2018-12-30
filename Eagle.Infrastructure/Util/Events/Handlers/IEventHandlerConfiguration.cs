
namespace Util.Events.Handlers
{
    /// <summary>
    /// 事件处理器配置
    /// </summary>
    public interface IEventHandlerConfiguration
    {
        /// <summary>
        /// 配置事件处理器
        /// </summary>
        ///// <typeparam name="TEvent">事件类型</typeparam>
        ///// <typeparam name="THandler">事件处理器类型</typeparam>
        /// <param name="queue">消息队列</param>
        /// <param name="concurrent">并发数量</param>
        /// <returns></returns>
        void ConfigureHandler<TEvent, THandler>(string queue, int concurrent = 5)
            where TEvent : Event
            where THandler : IEventHandler<TEvent>;
    }
}
