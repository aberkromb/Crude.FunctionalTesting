using Crude.FunctionalTesting.Dependencies;
using Crude.FunctionalTesting.Dependencies.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Crude.FunctionalTesting.TestServer
{
    public class WebApplicationFactoryBuilder<TStartup>
        : WebApplicationFactory<TStartup> where TStartup : class
    {
        private DependenciesBuilder _dependenciesBuilder;
        private RunningDependencies _runningDependencies;
        private IDependencyManager _dependencyManager;

        public WebApplicationFactoryBuilder<TStartup> AddDependenciesBuilder(
            DependenciesBuilder dependenciesBuilder)
        {
            _dependenciesBuilder = dependenciesBuilder;
            return this;
        }

        public IDependencyManager DependencyManager => _dependencyManager;

        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            
            return base.CreateWebHostBuilder();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            _runningDependencies = _dependenciesBuilder.Start();

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                _dependencyManager = _runningDependencies.ConfigureServices(context.Configuration, services);
            });
        }

        protected override void Dispose(bool disposing)
        {
            _dependenciesBuilder.Dispose();
            base.Dispose(disposing);
        }
    }
}