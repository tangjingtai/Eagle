using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Util.Events;
using Util.Helpers;

namespace Eagle.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.CancelKeyPress += async (s, e) =>
            {
                IEventBus bus;
                if (Ioc.TryCreate<IEventBus>(out bus))
                    await bus.StopEventBusAsync();
            };
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseStartup<Startup>();
        }
    }
}
