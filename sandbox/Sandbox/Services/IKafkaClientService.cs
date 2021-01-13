using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json.Linq;

namespace Sandbox.Services
{
    public interface IKafkaClientService
    {
        Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig);
        Task Produce(string topic, object message, CancellationToken cancellationToken);
        IEnumerable<JObject> Consume(string topic, TimeSpan? consumeTimeout = null);
    }
}