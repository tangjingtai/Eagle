using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Consul;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ruanmou.ConsulServiceRegistration;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Util.ServiceDiscovery
{
// consul服务注册扩展类
    public static class ConsulRegistrationExtensions
    {
        public static void AddConsul(this IServiceCollection services)
        {
            // 读取服务配置文件
            var config = new ConfigurationBuilder().AddJsonFile("service.config.json").Build();
            services.Configure<ConsulServiceOptions>(config);

            var option = config.Get<ConsulServiceOptions>();

            //var option = new ConsulServiceOptions();
            //config.Bind("", option);
            services.AddConsul(option);
        }

        public static void AddConsul(this IServiceCollection services, Action<ConsulServiceOptions> cfg)
        {
            if (cfg == null)
                throw new ArgumentNullException(nameof(cfg));
                       
            var options = new ConsulServiceOptions();
            cfg(options);
            services.AddConsul(options);
        }

        public static void AddConsul(this IServiceCollection services, ConsulServiceOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.ConsulAddress))
                throw new Exception("未配置consul地址");

            services.AddSingleton<ConsulServiceOptions>(options);

            var consulClient = new ConsulClient(configuration =>
            {
                //服务注册的地址，集群中任意一个地址
                configuration.Address = new Uri(options.ConsulAddress);
            });
            services.AddSingleton<ConsulClient>(consulClient);
        }

        public static IApplicationBuilder UseConsulHttp(this IApplicationBuilder app)
        {
            // 获取主机生命周期管理接口
            var lifetime =  app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 获取服务配置项
            var serviceOptions = GetConsulServiceOptions(app);
            var config =  app.ApplicationServices.GetRequiredService<IConfiguration>();
            var serviceNodeUri = GetServiceNodeUri(app);
            
            var consulClient = app.ApplicationServices.GetService<ConsulClient>();
            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceOptions.ServiceId, 
                Name = serviceOptions.ServiceName,// 服务名
                Address = serviceNodeUri.Host,
                
                Port = serviceNodeUri.Port, // 服务端口
                Check = new AgentServiceCheck
                {
                    // 注册超时
                    Timeout = TimeSpan.FromSeconds(5),   
                    // 服务停止多久后注销服务
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                    // 健康检查地址
                    HTTP = $"{serviceNodeUri.Scheme}://{serviceNodeUri.Host}:{serviceNodeUri.Port}{serviceOptions.HealthCheck}",
                    // 健康检查时间间隔
                    Interval = TimeSpan.FromSeconds(10),                    
                }
            };

            // 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;
        }

        public static IApplicationBuilder UseConsulGRPC(this IApplicationBuilder app)
        {
            // 获取主机生命周期管理接口
            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            // 获取服务配置项
            var serviceOptions = GetConsulServiceOptions(app);
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var serviceNodeUri = GetServiceNodeUri(app);

            var consulClient = app.ApplicationServices.GetService<ConsulClient>();
            // 节点服务注册对象
            var registration = new AgentServiceRegistration()
            {
                ID = serviceOptions.ServiceId,
                Name = serviceOptions.ServiceName,// 服务名
                Address = serviceNodeUri.Host,

                Port = serviceNodeUri.Port, // 服务端口
                Checks = new AgentServiceCheck[] {
                    new AgentServiceCheck
                    {
                        // 注册超时
                        Timeout = TimeSpan.FromSeconds(5),
                        // 服务停止多久后注销服务
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        // 健康检查地址
                        GRPC = $"{serviceNodeUri.Host}:{serviceNodeUri.Port}",
                        GRPCUseTLS = true,
                        // 健康检查时间间隔
                        Interval = TimeSpan.FromSeconds(10),
                        TLSSkipVerify = true
                    }
                }
            };

            // 注册服务
            consulClient.Agent.ServiceRegister(registration).Wait();

            // 应用程序终止时，注销服务
            lifetime.ApplicationStopping.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(serviceOptions.ServiceId).Wait();
            });

            return app;
        }

        private static ConsulServiceOptions GetConsulServiceOptions(IApplicationBuilder app)
        {
            // 获取服务配置项
            var serviceOptions = app.ApplicationServices.GetRequiredService<IOptions<ConsulServiceOptions>>().Value;
            
            // 服务ID必须保证唯一
            serviceOptions.ServiceId = Guid.NewGuid().ToString();
            return serviceOptions;
        }

        private static Uri GetServiceNodeUri(IApplicationBuilder app)
        {
            var config = app.ApplicationServices.GetRequiredService<IConfiguration>();
            // 使用参数配置服务注册地址
            var address = config["address"];
            if (string.IsNullOrEmpty(address))
            {
                // 获取当前服务地址和端口
                var features = app.Properties["server.Features"] as FeatureCollection;
                address = features?.Get<IServerAddressesFeature>().Addresses.First();
            }
            return new Uri(address);
        }
    }
}
