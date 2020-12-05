using System.Collections.Generic;

namespace Crude.FunctionalTesting.Dependencies
{
    /// <summary>
    ///     Менеджер созданных зависимостей 
    /// </summary>
    public interface IDependencyManager
    {
        /// <summary>
        ///     Получить все зависимости
        /// </summary>
        IEnumerable<IDependency> GetDependencies();
        
        /// <summary>
        ///     Получить созданную зависимость по ее типу.
        /// </summary>
        T GetDependency<T>() where T : IDependency;
    }
}