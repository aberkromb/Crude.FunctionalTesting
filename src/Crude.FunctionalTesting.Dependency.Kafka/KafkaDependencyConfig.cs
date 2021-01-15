using System;
using System.Collections.Generic;
using Crude.FunctionalTesting.Core.Dependencies;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaDependencyConfig : IDependencyConfig
    {
        /// <summary>
        ///     Имя образа
        /// </summary>
        public string Image { get; set; } = "confluentinc/cp-kafka:5.3.1";

        /// <summary>
        ///     Дополнительные переменные окружения если необходимо
        /// </summary>
        public IReadOnlyCollection<(string Key, string Value)> EnvironmentVariables { get; set; } =
            new List<(string Key, string Value)>
            {
                ("KAFKA_ADVERTISED_LISTENERS", $"INTERNAL://:29092,EXTERNAL://{GetAdvertisedListenerHost()}:9092"),
                ("KAFKA_LISTENERS", "INTERNAL://:29092,EXTERNAL://:9092"),
                ("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "INTERNAL:PLAINTEXT,EXTERNAL:PLAINTEXT"),
                ("KAFKA_INTER_BROKER_LISTENER_NAME", "INTERNAL"),
            };

        private static string GetAdvertisedListenerHost()
        {
            var kafkaAdvertisedListeners = Environment.GetEnvironmentVariable("DOCKER_CUSTOM_HOST_IP");

            return kafkaAdvertisedListeners switch
            {
                null => "localhost",
                "" => "localhost",
                _ => kafkaAdvertisedListeners
            };
        }

        /// <summary>
        ///     Открыть порты
        /// </summary>
        public uint ExposePort { get; set; } = 9092;

        /// <summary>
        ///     Имя зависимости
        /// </summary>
        public string DependencyName { get; set; } = "kafka-functional-test";

        /// <summary>
        ///     Переиспользовать если такая зависимость уже существует
        /// </summary>
        public bool ReuseDependencyIfExist { get; set; } = true;

        public static KafkaDependencyConfig Default => new KafkaDependencyConfig();
    }
}