using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Util.Logs.MSLogging
{
    public class LoggerFactory : ILoggerFactory
    {
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {

            return new Logger(Log.GetLog(categoryName));
        }

        public void Dispose()
        {
        }
    }
}
