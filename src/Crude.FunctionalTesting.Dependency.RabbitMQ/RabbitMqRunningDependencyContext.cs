using System;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ
{
    public class RabbitMqRunningDependencyContext : IRunningDependencyContext
    {
        private readonly IContainerService _container;
        private readonly RabbitMqDependencyConfig _config;

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; }
        
        public IDependencyConfig DependencyConfig => _config;

        public RabbitMqRunningDependencyContext(IConfiguration configuration,
                                             IServiceCollection services,
                                             IContainerService container,
                                             RabbitMqDependencyConfig dependencyConfig)
        {
            Configuration = configuration;
            Services = services;
            _container = container;
            _config = dependencyConfig;
        }

        public string GetDependencyAddress() =>
            Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") is null
                ? _container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString()
                : Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");
    }
}