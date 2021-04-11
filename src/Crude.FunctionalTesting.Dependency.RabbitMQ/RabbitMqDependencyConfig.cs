using System;
using System.Collections.Generic;
using Crude.FunctionalTesting.Core.Dependencies;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ
{
    public class RabbitMqDependencyConfig : IDependencyConfig
    {
        /// <summary>
        ///     Имя образа
        /// </summary>
        public string Image { get; set; } = "rabbitmq:3.8.14-management";

        /// <summary>
        ///     Дополнительные переменные окружения если необходимо
        /// </summary>
        public IReadOnlyCollection<(string Key, string Value)> EnvironmentVariables { get; set; } = 
            Array.Empty<(string Key, string Value)>();

        /// <summary>
        ///     Открыть порты
        /// </summary>
        public uint ExposePort { get; set; } = 5672;
        
        /// <summary>
        ///     UI порт
        /// </summary>
        public uint UiPort { get; set; } = 15672;

        /// <summary>
        ///     Имя зависимости
        /// </summary>
        public string DependencyName { get; set; } = "rabbitmq-functional-test";

        /// <summary>
        ///     Переиспользовать если такая зависимость уже существует
        /// </summary>
        public bool ReuseDependencyIfExist { get; set; } = true;

        public static RabbitMqDependencyConfig Default => new RabbitMqDependencyConfig();
    }
}