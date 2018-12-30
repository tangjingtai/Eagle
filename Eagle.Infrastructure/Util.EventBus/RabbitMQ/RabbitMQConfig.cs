using System;
using System.Collections.Generic;
using System.Text;

namespace Util.EventBus.RabbitMQ
{
    public class RabbitMQConfig
    {
        public string Host { get; set; }

        public string UserName { get; set;}

        public string Password { get; set; }
    }

    public class QueueConfig
    {
        public string Host { get; set; }

        public string Queue { get; set; }

        public Type EventType { get; set; }
    }
}
