using System;
using Microsoft.Extensions.Logging;
using Util.Logs.Abstractions;
using Util.Logs.Formats;

namespace Util.Logs.Log4net
{
    /// <summary>
    /// Log4日志提供程序
    /// </summary>
    public class Log4Provider : ILogProvider {
        /// <summary>
        /// log4net日志操作
        /// </summary>
        private readonly log4net.ILog _logger;
        /// <summary>
        /// 日志格式化器
        /// </summary>
        private readonly ILogFormat _format;

        /// <summary>
        /// 日志仓库名称
        /// </summary>
        public const string LOG_REPOSITORY_NAME = "Eagle.Log";

        /// <summary>
        /// 初始化日志
        /// </summary>
        /// <param name="logName">日志名称</param>
        /// <param name="format">日志格式化器</param>
        public Log4Provider( string logName, ILogFormat format = null ) {
            _logger = GetLogger( logName );
            _format = format;
        }

        /// <summary>
        /// 获取Log4net日志操作
        /// </summary>
        /// <param name="logName">日志名称</param>
        public static log4net.ILog GetLogger( string logName ) {
            return log4net.LogManager.GetLogger(LOG_REPOSITORY_NAME, logName );
        }

        /// <summary>
        /// 日志名称
        /// </summary>
        public string LogName => _logger.Logger.Name;

        /// <summary>
        /// 调试级别是否启用
        /// </summary>
        public bool IsDebugEnabled => _logger.IsDebugEnabled;

        /// <summary>
        /// 跟踪级别是否启用
        /// </summary>
        public bool IsTraceEnabled => false;

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="level">日志等级</param>
        /// <param name="content">日志内容</param>
        public void WriteLog( LogLevel level, ILogContent content ) {
            var provider = GetFormatProvider();
            switch(level)
            {
                case LogLevel.Debug:
                    _logger.DebugFormat(provider, "{0}", content);
                    return;
                case LogLevel.Information:
                    _logger.InfoFormat(provider, "{0}", content);
                    return;
                case LogLevel.Warning:
                    _logger.WarnFormat(provider, "{0}", content);
                    return;
                case LogLevel.Error:
                    _logger.ErrorFormat(provider, "{0}", content);
                    return;
                case LogLevel.Critical:
                    _logger.FatalFormat(provider, "{0}", content);
                    return;
                default:
                    return;
            }
        }

        /// <summary>
        /// 获取格式化提供程序
        /// </summary>
        private IFormatProvider GetFormatProvider() {
            if( _format == null )
                return null;
            return new FormatProvider( _format );
        }
    }
}
