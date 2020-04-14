using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Util.EventBus.RabbitMQ;
using Util.Events;
using Util.Events.Handlers;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EventBusExtension
    {
        /// <summary>
        /// 添加RabbitMQ作为事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddEventBusOfRabbitMQ(this IServiceCollection services)
        {
            services.AddScoped<IEventHandlerManager, EventHandlerManager>();
            //services.AddSingleton<IEventPublisher>(new EventPublisher(config.Host, config.UserName, config.Password));
            services.AddSingleton<IEventBus, EventBus>();
            return services;
        }

        /// <summary>
        /// 注册事件总线的处理器
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="registrator"></param>
        /// <returns></returns>
        public static void UseEventBusOfRabbitMQ(this IApplicationBuilder app, RabbitMQConfig config, Action<IEventHandlerConfiguration> handlerConfigurator)
        {
            var bus = app.ApplicationServices.GetService<IEventBus>();
            bus.SubscribeAsync(config.Host, config.UserName, config.Password, handlerConfigurator);
        }
    }
}