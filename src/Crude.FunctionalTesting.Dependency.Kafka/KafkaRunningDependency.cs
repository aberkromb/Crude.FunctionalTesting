using System;
using System.Threading;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly ICompositeService _compositeService;
        private readonly KafkaDependencyConfig _config;
        private KafkaRunningDependencyContext _context;

        public KafkaRunningDependency(Action<IRunningDependencyContext> configureServices,
                                      ICompositeService compositeService,
                                      KafkaDependencyConfig config)
        {
            _configureServices = configureServices;
            _compositeService = compositeService;
            _config = config;
        }


        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new KafkaRunningDependencyContext(configuration, services, _compositeService, _config);
            _configureServices(_context);
        }

        public Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken)
        {
            return Task.FromResult((IDependency) new KafkaDependency(_context));
        }

        public void Dispose()
        {
        }
    }
}