using System;
using System.IO;
using System.Linq;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaDependencyBuilder : IDependencyBuilder
    {
        private KafkaDependencyConfig _config;
        private Action<IRunningDependencyContext> _configureServices;
        private IContainerService _container;

        public IDependencyBuilder AddConfig(IDependencyConfig dependencyConfig)
        {
            _config = (KafkaDependencyConfig) dependencyConfig;

            return this;
        }

        public IDependencyBuilder AddConfigureServices(Action<IRunningDependencyContext> configureServices)
        {
            _configureServices = configureServices;

            return this;
        }

        public IRunningDependency Start()
        {
            var builder = BuildContainer();

            var container = builder.Build().Start();

            return new KafkaRunningDependency(_configureServices, container, _config);
        }

        private ContainerBuilder BuildContainer()
        {
            var builder = new Builder()
                          .UseContainer()
                          .UseImage(_config.Image)
                          .WithEnvironment(_config.EnvironmentVariables
                                                  .Select(s => $"{s.Key}={s.Value}")
                                                  .ToArray())
                          .ExposePort((int) _config.ExposePort, (int) _config.ExposePort)
                          .WaitForPort($"{_config.ExposePort.ToString()}/tcp", 30000 /*30s*/, Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") ?? "127.0.0.1")
                          .WithName(_config.DependencyName);

            if (_config.ReuseDependencyIfExist)
                builder.ReuseIfExists();

            return builder;
        }
    }
}