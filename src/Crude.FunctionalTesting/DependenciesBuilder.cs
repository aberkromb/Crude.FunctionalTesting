using System;
using System.Collections.Generic;
using System.Linq;
using Crude.FunctionalTesting.Dependencies;
using Crude.FunctionalTesting.Dependencies.Common;

namespace Crude.FunctionalTesting
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

        internal RunningDependencies Start()
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
            var c = _runningDependencies;
            _runningDependencies = null;
            try
            {
                foreach (var dependency in _runningDependencies)
                    dependency.Dispose();
            }
            catch
            {
                // ignore
            }
        }
    }
}