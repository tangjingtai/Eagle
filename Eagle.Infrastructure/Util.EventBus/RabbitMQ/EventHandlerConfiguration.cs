using MassTransit.RabbitMqTransport;
using System;
using Util.Events;
using Util.Events.Handlers;
using GreenPipes;
using MassTransit;
using Util.Helpers;

namespace Util.EventBus.RabbitMQ
{
    /// <summary>
    /// 使用RabbitMQ作为事件总线，事件处理器配置
    /// </summary>
    public class EventHandlerConfiguration : IEventHandlerConfiguration
    {
        IRabbitMqBusFactoryConfigurator _busConfigurator;
        IRabbitMqHost _mqhost;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="busConfigurator"></param>
        /// <param name="mqhost"></param>
        internal EventHandlerConfiguration(IRabbitMqBusFactoryConfigurator busConfigurator, IRabbitMqHost mqhost)
        {
            _busConfigurator = busConfigurator;
            _mqhost = mqhost;
        }

        /// <summary>
        /// 配置事件处理器
        /// </summary>
        ///// <typeparam name="TEvent">事件类型</typeparam>
        ///// <typeparam name="THandler">事件处理器类型</typeparam>
        /// <param name="queue">消息队列</param>
        /// <param name="concurrent">并发数量</param>
        /// <returns></returns>
        public void ConfigureHandler<TEvent, THandler>(string queue, int concurrent = 5)
            where TEvent : Event
            where THandler : IEventHandler<TEvent>
        {
            _busConfigurator.ReceiveEndpoint(_mqhost, queue, endpoint =>
            {
                endpoint.UseRetry(retryConfig => retryConfig.Interval(1, new TimeSpan(0, 2, 0)));
                endpoint.PrefetchCount = (ushort)concurrent;
                endpoint.Durable = true;
                endpoint.AutoDelete = false;
                endpoint.Handler<TEvent>(async context =>
                {
                    try
                    {
                        await Ioc.Create<IEventHandler<TEvent>>().HandleAsync(context.Message);
                    }
                    catch (StoppedConsumeException)
                    {
                        // 消费者停止消费消息后，将异常抛出，消息重试或挪到error队列中
                        // 等待下次处理
                        throw;
                    }
                    catch (Exception ex)
                    {
                        //Log.Write(string.Format("执行{0}错误", typeof(THandler)), MessageType.Error, this.GetType(), ex);
                    }
                });
            });
        }
    }
}
