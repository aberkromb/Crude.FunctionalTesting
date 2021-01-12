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
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public PostgresRunningDependencyContext(PostgresDependencyConfig config,
            IContainerService container,
            IConfiguration configuration,
            IServiceCollection services)
        {
            _config = config;
            _container = container;
            _configuration = configuration;
            _services = services;
        }

        public IConfiguration Configuration => _configuration;

        public IServiceCollection Services => _services;

        public IDependencyConfig DependencyConfig => _config;

        /// <summary>
        ///     Отдает строку подключения для postgres
        /// </summary>
        public string ConnectionString =>
            $"Host={GetHost()}; Port={_config.ExposePort}; Database={_config.Database}; Username={_config.UserName}; Password={_config.Password}";

        private string GetHost() => 
            Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") is null
            ? _container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString()
            : Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");
    }
}