using Confluent.Kafka;
using Crude.FunctionalTesting.Core.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaRunningDependencyContext : IRunningDependencyContext
    {
        private readonly KafkaDependencyConfig _config;

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        
        public IDependencyConfig DependencyConfig => _config;

        public KafkaRunningDependencyContext(IConfiguration configuration, IServiceCollection services, KafkaDependencyConfig dependencyConfig)
        {
            Configuration = configuration;
            Services = services;
            _config = dependencyConfig;
        }

        public ClientConfig GetClientConfig() => new ClientConfig
        {
            BootstrapServers = ""
        };
    }
}