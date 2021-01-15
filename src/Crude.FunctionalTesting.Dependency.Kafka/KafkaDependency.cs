using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Crude.FunctionalTesting.Core.Dependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Crude.FunctionalTesting.Dependency.Kafka
{
    public class KafkaDependency : IDependency
    {
        private readonly KafkaRunningDependencyContext _context;

        public KafkaDependency(KafkaRunningDependencyContext context)
        {
            _context = context;
        }

        static async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
        {
            using (var adminClient = new AdminClientBuilder(cloudConfig).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                    {
                        new TopicSpecification {Name = name, NumPartitions = numPartitions, ReplicationFactor = replicationFactor}
                    });
                }
                catch (CreateTopicsException e)
                {
                    if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine("Topic already exists");
                    }
                }
            }
        }

        public async Task Produce(string topic, object message)
        {
            CreateTopicMaybe(topic, 1, 1, _context.GetClientConfig()).GetAwaiter().GetResult();

            using var producer = new ProducerBuilder<string, string>(_context.GetClientConfig()).Build();

            var val = JObject.FromObject(message).ToString(Formatting.None);

            await producer.ProduceAsync(topic,
                                        new Message<string, string> {Key = null, Value = val},
                                        CancellationToken.None);

            producer.Flush(TimeSpan.FromSeconds(10));
        }

        public IEnumerable<JObject> Consume(string topic, TimeSpan? consumeTimeout = null)
        {
            var consumerConfig = new ConsumerConfig(_context.GetClientConfig());
            consumerConfig.GroupId = "dotnet-example-group-1";
            consumerConfig.AutoOffsetReset = AutoOffsetReset.Earliest;
            consumerConfig.EnableAutoCommit = false;

            var result = new List<JObject>();

            using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            consumer.Subscribe(topic);
            try
            {
                var isEnd = false;
                do
                {
                    var cr = consumer.Consume(consumeTimeout ?? TimeSpan.FromMilliseconds(5000));
                    if (cr != null)
                    {
                        consumer.Commit(cr);
                        result.Add(JObject.Parse(cr.Message.Value));
                    }
                    else
                    {
                        isEnd = true;
                    }
                } while (isEnd is false);
            }
            finally
            {
                consumer.Close();
            }

            return result;
        }
    }
}