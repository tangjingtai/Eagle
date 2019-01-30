
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Util.Logs.Abstractions;
using Util.Logs.Extensions;

namespace Util.Logs.MSLogging
{
    public class Logger : ILogger
    {
        private ILog _logger;

        public Logger(ILog logger)
        {
            _logger = logger;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
                return _logger.IsDebugEnabled;
            if (logLevel == LogLevel.Trace)
                return _logger.IsTraceEnabled;
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
                throw new ArgumentNullException(nameof(formatter));
            var message = formatter(state, exception);
            if (exception != null)
                _logger.Exception(exception);

            _logger.Set<ILogContent>(content =>
            {
                if (eventId != null)
                {
                    content.TraceId = eventId.Id.ToString();
                }
                content.Content(message);
            });
            switch(logLevel)
            {
                case LogLevel.Trace:
                    _logger.Trace();
                    break;
                case LogLevel.Debug:
                    _logger.Debug();
                    break;
                case LogLevel.Information:
                    _logger.Info();
                    break;
                case LogLevel.Warning:
                    _logger.Warn();
                    break;
                case LogLevel.Error:
                    _logger.Error();
                    break;
                case LogLevel.Critical:
                    _logger.Fatal();
                    break;
            }
        }
        
    }
}
