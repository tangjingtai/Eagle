using System;
using System.IO;
using System.Reflection;
using Exceptionless;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Util.Logs.Abstractions;
using Util.Logs.Core;
using Util.Logs.Formats;
using Util.Logs.Log4net;

namespace Util.Logs.Extensions {
    /// <summary>
    /// 日志扩展
    /// </summary>
    public static partial class Extensions
    { 
        /// <summary>
        /// 注册NLog日志操作
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void AddNLog( this IServiceCollection services ) {
            services.TryAddScoped<ILogProviderFactory, Util.Logs.NLog.NLogProviderFactory>();
            services.TryAddSingleton<ILogFormat, ContentFormat>();
            services.TryAddScoped<ILogContext, LogContext>();
            services.TryAddScoped<ILog, Log>();
        }

        /// <summary>
        /// 注册Log4net日志操作
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="log4NetConfigFile">log4net配置文件路径</param>
        public static void AddLog4net(this IServiceCollection services, string log4NetConfigFile = "log4net.config")
        {
            services.TryAddScoped<ILogProviderFactory, Log4ProviderFactory>();
            services.TryAddSingleton<ILogFormat, ContentFormat>();
            services.TryAddScoped<ILogContext, LogContext>();
            services.TryAddScoped<ILog, Log>();

            var fileNamePath = log4NetConfigFile;
            if (!Path.IsPathRooted(fileNamePath))
            {
				if (!File.Exists(fileNamePath))
				{
					fileNamePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), fileNamePath);
				}
            }
            fileNamePath = Path.GetFullPath(fileNamePath);

            var repository = LogManager.CreateRepository(Log4Provider.LOG_REPOSITORY_NAME);
            log4net.Config.XmlConfigurator.ConfigureAndWatch(repository, new FileInfo(fileNamePath));
        }

        /// <summary>
        /// 注册Exceptionless日志操作
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configAction">配置操作</param>
        public static void AddExceptionless( this IServiceCollection services, Action<ExceptionlessConfiguration> configAction ) {
            services.TryAddScoped<ILogProviderFactory, Util.Logs.Exceptionless.LogProviderFactory>();
            services.TryAddSingleton( typeof( ILogFormat ), t => NullLogFormat.Instance );
            services.TryAddScoped<ILogContext, Util.Logs.Exceptionless.LogContext>();
            services.TryAddScoped<ILog, Log>();
            configAction?.Invoke( ExceptionlessClient.Default.Configuration );
        }
    }
}
