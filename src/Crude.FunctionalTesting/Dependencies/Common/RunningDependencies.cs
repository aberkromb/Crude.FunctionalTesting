using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Dependencies.Common
{
    internal class RunningDependencies
    {
        private readonly IEnumerable<IRunningDependency> _runningDependencies;

        public RunningDependencies(IEnumerable<IRunningDependency> runningDependencies)
        {
            _runningDependencies = runningDependencies;
        }

        public IDependencyManager ConfigureServices(IConfiguration configuration,
                                                    IServiceCollection services)
        {
            foreach (var runningDependency in _runningDependencies)
                runningDependency.ConfigureService(configuration, services);

            var dependencies = AfterDependenciesStart(_runningDependencies);

            return new DependencyManager(dependencies);
        }

        private static IEnumerable<IDependency> AfterDependenciesStart(
            IEnumerable<IRunningDependency> runningDependencies)
        {
            return Task.WhenAll(runningDependencies.Select(dependency =>
                dependency.AfterDependencyStart(CancellationToken.None))).GetAwaiter().GetResult();
        }
    }
}