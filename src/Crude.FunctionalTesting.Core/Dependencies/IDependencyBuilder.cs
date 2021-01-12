using System;

namespace Crude.FunctionalTesting.Core.Dependencies
{
    /// <summary>
    ///     Интерфейс для создания зависимостей
    /// </summary>
    public interface IDependencyBuilder
    {
        /// <summary>
        ///     Добавить конфиг зависимости
        /// </summary>
        IDependencyBuilder AddConfig(IDependencyConfig dependencyConfig);

        /// <summary>
        ///     Добавить конфигуратор контейнера
        /// </summary>
        //TODO вынести в extensions
        IDependencyBuilder AddConfigureServices(Action<IRunningDependencyContext> configureServices);
        
        /// <summary>
        ///     Запускает зависимость
        /// </summary>
        IRunningDependency Start();
    }
}