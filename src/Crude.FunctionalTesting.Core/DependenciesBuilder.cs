using System;
using System.Collections.Generic;
using System.Linq;
using Crude.FunctionalTesting.Core.Dependencies;

namespace Crude.FunctionalTesting.Core
{
    public class DependenciesBuilder : IDisposable
    {
        private IEnumerable<IRunningDependency> _runningDependencies;
        private readonly List<IDependencyBuilder> _dependencyBuilders;

        public DependenciesBuilder()
        {
            _dependencyBuilders = new List<IDependencyBuilder>();
            _runningDependencies = Enumerable.Empty<IRunningDependency>();
        }

        public RunningDependencies Start()
        {
            return new RunningDependencies(_dependencyBuilders.Select(builder => builder.Start()).ToList());
        }

        public DependenciesBuilder AddDependency(IDependencyBuilder dependencyBuilder)
        {
            _dependencyBuilders.Add(dependencyBuilder);

            return this;
        }

        public void Dispose()
        {
            var captureRunningDependencies = _runningDependencies;
            _runningDependencies = null;
            try
            {
                foreach (var dependency in captureRunningDependencies)
                    dependency.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }
}