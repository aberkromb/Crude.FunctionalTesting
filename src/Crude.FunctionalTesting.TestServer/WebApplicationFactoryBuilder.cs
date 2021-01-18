using System;
using System.Collections.Generic;
using Crude.FunctionalTesting.Core;
using Crude.FunctionalTesting.Core.Dependencies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Crude.FunctionalTesting.TestServer
{
    public class WebApplicationFactoryBuilder<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private DependenciesBuilder _dependenciesBuilder;
        private RunningDependencies _runningDependencies;
        private List<Action<IWebHostBuilder>> _webHostBuilderConfigurator = new List<Action<IWebHostBuilder>>();

        public WebApplicationFactoryBuilder<TStartup> AddWebHostBuilderConfigurator(Action<IWebHostBuilder> configurator)
        {
            _webHostBuilderConfigurator.Add(configurator);
            return this;
        }
        
        public WebApplicationFactoryBuilder<TStartup> AddDependenciesBuilder(
            DependenciesBuilder dependenciesBuilder)
        {
            _dependenciesBuilder = dependenciesBuilder;
            return this;
        }

        public IDependencyManager DependencyManager { get; private set; }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            _runningDependencies = _dependenciesBuilder.Start();

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                DependencyManager = _runningDependencies.ConfigureServices(context.Configuration, services);
            });

            base.ConfigureWebHost(builder);
        }

        protected override void Dispose(bool disposing)
        {
            _dependenciesBuilder.Dispose();
            base.Dispose(disposing);
        }
    }
}