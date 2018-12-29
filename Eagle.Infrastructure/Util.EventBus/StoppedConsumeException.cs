using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Util.EventBus
{
    public class StoppedConsumeException : ApplicationException
    {
        public StoppedConsumeException()
        {

        }

        public StoppedConsumeException(string message)
            : base(message)
        {

        }
        public StoppedConsumeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}