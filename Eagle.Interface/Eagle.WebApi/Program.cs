using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Util.Events;
using Util.Helpers;
using Util.Logs;

namespace Eagle.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += async (s, e) =>
            {
                Log.GetLog(typeof(Program).FullName).Warn("ctrl + c 退出程序");
                IEventBus bus;
                if (Ioc.TryCreate<IEventBus>(out bus))
                    await bus.StopEventBusAsync();
            };
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                Log.GetLog(typeof(Program).FullName).Warn("程序退出");
            };
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, conf) => {
                    conf.AddJsonFile("appsettings.json", true);
                    conf.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
                    conf.SetBasePath(Directory.GetCurrentDirectory());
                    conf.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
            //return WebHost.CreateDefaultBuilder(args)
            //    .ConfigureAppConfiguration((context, conf)=> {
            //        conf.AddJsonFile("appsettings.json", true);
            //        conf.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
            //        conf.SetBasePath(Directory.GetCurrentDirectory());
            //        conf.AddCommandLine(args); 
            //    })
            //    .UseKestrel()
            //    .UseStartup<Startup>();
        }
    }
}
