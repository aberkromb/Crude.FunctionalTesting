using System;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Postgres
{
    public class PostgresRunningDependencyContext : IRunningDependencyContext
    {
        private readonly PostgresDependencyConfig _config;
        private readonly IContainerService _container;

        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; }

        public IDependencyConfig DependencyConfig => _config;

        public PostgresRunningDependencyContext(PostgresDependencyConfig config,
                                                IContainerService container,
                                                IConfiguration configuration,
                                                IServiceCollection services)
        {
            _config = config;
            _container = container;
            Configuration = configuration;
            Services = services;
        }

        /// <summary>
        ///     Отдает строку подключения для postgres
        /// </summary>
        public string ConnectionString =>
            $"Host={GetDependencyAddress()}; Port={_config.ExposePort}; Database={_config.Database}; Username={_config.UserName}; Password={_config.Password}";

        private string GetDependencyAddress() =>
            Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") is null
                ? _container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString()
                : Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");
    }
}