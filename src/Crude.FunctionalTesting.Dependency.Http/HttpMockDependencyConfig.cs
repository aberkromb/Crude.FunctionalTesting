using System.Collections.Generic;
using Crude.FunctionalTesting.Dependencies;

namespace Crude.FunctionalTesting.Dependency.Http
{
    /// <summary>
    ///     Кофиг зависимости для http моков
    /// </summary>
    public class HttpMockDependencyConfig : IDependencyConfig
    {
        /// <summary>
        ///     Имя образа
        /// </summary>
        public string Image { get; set; } = "bbyars/mountebank";

        /// <summary>
        ///     Дополнительные переменные окружения если необходимо
        /// </summary>
        public IReadOnlyCollection<(string Key, string Value)> EnvironmentVariables { get; set; } =
            new List<(string Key, string Value)>();

        /// <summary>
        ///     Открыть порт для мокинга
        /// </summary>
        public uint ExposeApiPort { get; set; } = 25251;
        
        /// <summary>
        ///     Открыть пор для UI
        /// </summary>
        public uint ExposeUiPort { get; set; } = 2525;

        /// <summary>
        ///     Имя зависимости
        /// </summary>
        public string DependencyName { get; set; } = "httpmock-integration-test";
        
        /// <summary>
        ///     Переиспользовать если такая зависимость уже существует
        /// </summary>
        public bool ReuseDependencyIfExist { get; set; } = true;

        
        public static HttpMockDependencyConfig Default => new HttpMockDependencyConfig();
    }
}