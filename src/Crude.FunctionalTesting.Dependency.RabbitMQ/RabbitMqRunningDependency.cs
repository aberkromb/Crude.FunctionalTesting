using System;
using System.Threading;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ
{
    public class RabbitMqRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly IContainerService _compositeService;
        private readonly RabbitMqDependencyConfig _config;
        private RabbitMqRunningDependencyContext _context;

        public RabbitMqRunningDependency(
            Action<IRunningDependencyContext> configureServices,
            IContainerService compositeService,
            RabbitMqDependencyConfig config)
        {
            _configureServices = configureServices;
            _compositeService = compositeService;
            _config = config;
        }


        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new RabbitMqRunningDependencyContext(configuration, services, _compositeService, _config);
            _configureServices(_context);
        }

        public Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken)
        {
            return Task.FromResult((IDependency) new RabbitMqDependency(_context));
        }

        public void Dispose()
        {
        }
    }
}