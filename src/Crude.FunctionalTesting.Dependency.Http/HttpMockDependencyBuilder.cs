using System;
using System.Linq;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Crude.FunctionalTesting.Dependency.Http
{
    public class HttpMockDependencyBuilder : IDependencyBuilder
    {
        private HttpMockDependencyConfig _config;
        private IContainerService _container;
        private Action<IRunningDependencyContext> _configureServices;

        public IDependencyBuilder AddConfig(IDependencyConfig dependencyConfig)
        {
            _config = (HttpMockDependencyConfig) dependencyConfig;

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

            _container = builder.Build().Start();

            return new HttpMockRunningDependency(_configureServices, _container, _config);
        }

        private ContainerBuilder BuildContainer()
        {
            var builder = new Builder()
                .UseContainer()
                .UseImage(_config.Image)
                .WithEnvironment(
                    _config.EnvironmentVariables
                        .Select(s => $"{s.Key}={s.Value}")
                        .ToArray())
                .ExposePort((int) _config.ExposeApiPort, (int) _config.ExposeApiPort)
                .ExposePort((int) _config.ExposeUiPort, (int) _config.ExposeUiPort)
                // .WaitForPort($"{_config.ExposeApiPort.ToString()}/tcp", 30000 /*30s*/)
                // .Command("mb", "start")
                .WithName(_config.DependencyName);

            if (_config.ReuseDependencyIfExist)
                builder.ReuseIfExists();
            
            return builder;
        }
    }
}