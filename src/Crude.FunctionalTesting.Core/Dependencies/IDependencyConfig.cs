using System;

namespace Crude.FunctionalTesting.Core.Dependencies
{
    /// <summary>
    ///     Маркерный интерфейс для конфигурации зависимостей
    /// </summary>
    public interface IDependencyConfig
    {
        string DockerHost => Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP") ?? "localhost";
    }
}