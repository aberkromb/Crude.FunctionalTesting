using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crude.FunctionalTesting.Core.Dependencies
{
    public interface IRunningDependencyContext
    {
        public IConfiguration Configuration { get; }

        public IServiceCollection Services { get; }
        
        public IDependencyConfig DependencyConfig { get; }
    }
}