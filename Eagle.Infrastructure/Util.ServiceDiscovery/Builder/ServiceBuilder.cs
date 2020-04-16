using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Util.ServiceDiscovery.LoadBalancer;

namespace Util.ServiceDiscovery.Builder
{
    public class ServiceBuilder : IServiceBuilder
    {
        public IServiceProvider ServiceProvider { get; set; }

        public string ServiceName { get; set; }

        public string UriScheme { get; set; }

        public ILoadBalancer LoadBalancer  { get; set; }

        public ServiceBuilder(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public async Task<Uri> BuildAsync(string path)
        {
            var serviceList = await ServiceProvider.GetServicesAsync(ServiceName);
            var service = LoadBalancer.Resolve(serviceList);
            var baseUri = new Uri($"{UriScheme}://{service}");
            var uri = new Uri(baseUri, path);
            return uri;
        }
    }
}
