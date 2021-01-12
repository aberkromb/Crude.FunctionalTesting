using System;
using System.Threading;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly KafkaDependencyConfig _config;
        private KafkaRunningDependencyContext _context;

        public KafkaRunningDependency(Action<IRunningDependencyContext> configureServices,
                                            KafkaDependencyConfig config)
        {
            _configureServices = configureServices;
            _config = config;
        }


        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new KafkaRunningDependencyContext(configuration, services, _config);
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