using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Util.ServiceDiscovery.LoadBalancer
{
    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly object _lock = new object();
        private int _index = 0;

        public string Resolve(IList<string> services)
        {
            // 使用lock控制并发
            lock (_lock)
            {
                if (_index >= services.Count)
                {
                    _index = 0;
                }
                return services[_index++];
            }
        }
    }
}
