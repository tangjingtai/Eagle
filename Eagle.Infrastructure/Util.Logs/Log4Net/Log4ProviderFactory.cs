using Util.Logs.Abstractions;

namespace Util.Logs.Log4net
{
    /// <summary>
    /// Log4net日志提供程序工厂
    /// </summary>
    public class Log4ProviderFactory : ILogProviderFactory {
        /// <summary>
        /// 创建日志提供程序
        /// </summary>
        /// <param name="logName">日志名称</param>
        /// <param name="format">日志格式化器</param>
        public ILogProvider Create( string logName, ILogFormat format = null ) {
            return new Log4Provider( logName, format );
        }
    }
}
