using System;
using System.Linq;
using Confluent.Kafka;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaRunningDependencyContext : IRunningDependencyContext
    {
        private readonly ICompositeService _container;
        private readonly KafkaDependencyConfig _config;

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        public IDependencyConfig DependencyConfig => _config;

        public KafkaRunningDependencyContext(IConfiguration configuration,
                                             IServiceCollection services,
                                             ICompositeService container,
                                             KafkaDependencyConfig dependencyConfig)
        {
            Configuration = configuration;
            Services = services;
            _container = container;
            _config = dependencyConfig;
        }

        public ClientConfig GetClientConfig() => new ClientConfig()
        {
            BootstrapServers = $"{GetDependencyAddress()}:{_config.ExposePort}"
        };

        private string GetDependencyAddress() =>
            Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") is null
                ? _container.Containers
                            .First(c => c.Image.Name.Contains("cp-kafka", StringComparison.InvariantCultureIgnoreCase))
                            .ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString()
                : Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");
    }
}