using System;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    public class OracleRunningDependencyContext : IRunningDependencyContext
    {
        private readonly OracleDependencyConfig _config;
        private readonly IContainerService _container;

        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; }

        public IDependencyConfig DependencyConfig => _config;

        public OracleRunningDependencyContext(OracleDependencyConfig config,
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
        ///     Отдает строку подключения к БД
        /// </summary>
        public string ConnectionString =>
            $"Data Source={GetDependencyAddress()}:{_config.ExposePort}/ORCLCDB; User ID=FUNCTIONALTESTING; Password={_config.Password}";
//User ID=myUsername;Password=myPassword;Host=ora;Pooling=true;Min Pool Size=0;Max Pool Size=100;Connection Lifetime=0;

        public string GetDependencyAddress() =>
            Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") is null
                ? _container.ToHostExposedEndpoint($"{_config.ExposePort}/tcp").Address.ToString()
                : Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");
    }
}