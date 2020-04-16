namespace Ruanmou.ConsulServiceRegistration
{
    // Consul配置模型类
    public class ConsulServiceOptions
    {
        // 服务注册地址（Consul的地址，如果是集群，取任意一个地址即可）
        public string ConsulAddress { get; set; }

        // 服务ID
        public string ServiceId { get; set; }

        // 服务名称
        public string ServiceName { get; set; }

        // 健康检查地址
        public string HealthCheck { get; set; }
    }
}