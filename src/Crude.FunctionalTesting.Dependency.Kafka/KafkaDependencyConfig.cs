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
                ("KAFKA_BROKER_ID", 1.ToString()),
                ("KAFKA_LISTENER_SECURITY_PROTOCOL_MAP", "PLAINTEXT:PLAINTEXT,PLAINTEXT_HOST:PLAINTEXT"),
                ("KAFKA_INTER_BROKER_LISTENER_NAME", "PLAINTEXT"),
                ("KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR", 1.ToString()),
                ("KAFKA_AUTO_CREATE_TOPICS_ENABLE", "true"),
                ("KAFKA_HEAP_OPTS", "-Xmx512M"),
                ("KAFKA_ADVERTISED_LISTENERS", $"PLAINTEXT://kafka:29092,PLAINTEXT_HOST://{GetAdvertisedListenersPlainTextHost()}:9092"),
                ("KAFKA_ZOOKEEPER_CONNECT", $"localhost:2181")
            };

        private static string GetAdvertisedListenersPlainTextHost()
        {
            var kafkaAdvertisedListeners = Environment.GetEnvironmentVariable("KAFKA_ADVERTISED_LISTENERS_HOST");
            
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