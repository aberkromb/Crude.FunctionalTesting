using System.Collections.Generic;

namespace Crude.FunctionalTesting.Core.Dependencies.Implementations
{
    internal class DependencyManager : IDependencyManager
    {
        private readonly IEnumerable<IDependency> _dependencies;

        public DependencyManager(IEnumerable<IDependency> dependencies)
        {
            _dependencies = dependencies;
        }

        public IEnumerable<IDependency> GetDependencies()
        {
            return _dependencies;
        }

        public T GetDependency<T>() where T : IDependency
        {
            foreach (var dependency in _dependencies)
            {
                if (dependency is T typedDependency)
                    return typedDependency;
            }

            return default;
        }
    }
}