using System;
using System.IO;
using System.Linq;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Model.Common;
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
                          .ExposePort((int) _config.ExposePort2, (int) _config.ExposePort2)
                          // .WaitForPort($"{_config.ExposePort.ToString()}/tcp", 30000 /*30s*/)
                          .WithName(_config.DependencyName);

            if (_config.ReuseDependencyIfExist)
                builder.ReuseIfExists();
            //
            // var zookeeperBuilder = new Builder()
            //     .UseContainer()
            //     .UseImage("confluentinc/cp-zookeeper:5.3.1")
            //     .WithEnvironment("ZOOKEEPER_CLIENT_PORT=2181", "ZOOKEEPER_TICK_TIME=2000", "ZOOKEEPER_SYNC_LIMIT=2")
            //     .ExposePort(2181)
            //     .WithName("zookeeper-functional-tests")
            //     .UseNetwork("kafka-network")
            //     .ReuseIfExists();

            // var file = Path.Combine(Directory.GetCurrentDirectory(), (TemplateString) "docker-compose.yml");
            //
            // var fileContent = File.ReadAllText(file);
            //
            // var builder = new Builder()
            //     .UseContainer()
            //     .UseCompose()
            //     .FromFile(file)
            //     .RemoveOrphans();

            return builder;
        }
    }
}