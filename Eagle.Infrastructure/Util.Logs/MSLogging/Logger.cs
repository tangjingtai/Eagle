
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
        /// <summary>
        /// The logger
        /// </summary>
        private ILog _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Logger"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public Logger(ILog logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="state">The identifier for the scope.</param>
        /// <returns>An IDisposable that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel" /> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.Debug)
                return _logger.IsDebugEnabled;
            if (logLevel == LogLevel.Trace)
                return _logger.IsTraceEnabled;
            return true;
        }

        /// <summary>
        /// Writes a log entry.
        /// </summary>
        /// <typeparam name="TState">The type of the t state.</typeparam>
        /// <param name="logLevel">Entry will be written on this level.</param>
        /// <param name="eventId">Id of the event.</param>
        /// <param name="state">The entry to be written. Can be also an object.</param>
        /// <param name="exception">The exception related to this entry.</param>
        /// <param name="formatter">Function to create a <c>string</c> message of the <paramref name="state" /> and <paramref name="exception" />.</param>
        /// <exception cref="System.ArgumentNullException">formatter</exception>
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

    public class Logger<TCategoryName> : Logger, ILogger<TCategoryName>
    {
        public Logger() : base(Util.Logs.Log.GetLog())
        {
        }
    }
}
