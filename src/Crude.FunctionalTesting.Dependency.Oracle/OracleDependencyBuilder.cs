using System;
using System.Linq;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    public class OracleDependencyBuilder : IDependencyBuilder
    {
        private OracleDependencyConfig _config;
        private IContainerService _container;
        private Action<IRunningDependencyContext> _configureServices;

        public OracleDependencyBuilder()
        {
            _config = OracleDependencyConfig.Default;
        }

        public IDependencyBuilder AddConfig(IDependencyConfig dependencyConfig)
        {
            _config = (OracleDependencyConfig) dependencyConfig;

            return this;
        }

        //TODO вынести в extensions
        public IDependencyBuilder AddConfigureServices(Action<IRunningDependencyContext> configureServices)
        {
            _configureServices = configureServices;

            return this;
        }

        public IRunningDependency Start()
        {
            var builder = BuildContainer();

            _container = builder.Build().Start();

            return new OracleRunningDependency(_configureServices, _container, _config);
        }

        private ContainerBuilder BuildContainer()
        {
            var builder = new Builder()
                          .UseContainer()
                          .UseImage(_config.Image)
                          .WithEnvironment(_config.EnvironmentVariables
                                                  .Select(s => $"{s.Key}={s.Value}")
                                                  .Concat(new[]
                                                  {
                                                      $"ORACLE_PWD={_config.Password}",
                                                  })
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