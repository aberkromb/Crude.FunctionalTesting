using System.Collections.Generic;
using Crude.FunctionalTesting.Core.Dependencies;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    /// <summary>
    ///     Конфигурирование зависимости postgres
    /// </summary>
    public class OracleDependencyConfig : IDependencyConfig
    {
        /// <summary>
        ///     Имя образа
        /// </summary>
        public string Image { get; set; } = "hcfb/oracle:19.3.0-ee-for-ft";

        /// <summary>
        ///     Дополнительные переменные окружения если необходимо
        /// </summary>
        public IReadOnlyCollection<(string Key, string Value)> EnvironmentVariables { get; set; } =
            new List<(string Key, string Value)>();

        /// <summary>
        ///     Открыть порты
        /// </summary>
        public uint ExposePort { get; set; } = 1521;

        /// <summary>
        ///     Пароль для доступа к БД
        /// </summary>
        public string Password { get; set; } = "123";

        /// <summary>
        ///     Имя зависимости
        /// </summary>
        public string DependencyName { get; set; } = "oracle-functional-test";

        /// <summary>
        ///     Переиспользовать если такая зависимость уже существует
        /// </summary>
        public bool ReuseDependencyIfExist { get; set; } = true;

        public static OracleDependencyConfig Default => new OracleDependencyConfig();
    }
}