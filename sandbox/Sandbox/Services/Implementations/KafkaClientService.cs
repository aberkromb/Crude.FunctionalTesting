using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sandbox.Services.Implementations
{
    internal class KafkaClientService : IKafkaClientService
    {
        private readonly ClientConfig _clientConfig;
        private readonly ILogger<KafkaClientService> _logger;
        private readonly ProducerBuilder<string, string> _producerBuilder;


        public KafkaClientService(IOptions<ClientConfig> clientConfig, ILogger<KafkaClientService> logger)
        {
            _logger = logger;
            _clientConfig = clientConfig.Value;

            _producerBuilder = new ProducerBuilder<string, string>(_clientConfig);
        }

        public async Task CreateTopicMaybe(string name, int numPartitions, short replicationFactor, ClientConfig cloudConfig)
        {
            using var adminClient = new AdminClientBuilder(cloudConfig).Build();

            try
            {
                await adminClient.CreateTopicsAsync(new List<TopicSpecification>
                                                    {
                                                        new()
                                                        {
                                                            Name = name,
                                                            NumPartitions = numPartitions,
                                                            ReplicationFactor = replicationFactor
                                                        }
                                                    }
                                                   );
            }
            catch (CreateTopicsException e)
            {
                if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    _logger.LogError($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                }
                else
                {
                    _logger.LogInformation("Topic already exists");
                }
            }
        }

        public async Task Produce(string topic, object message, CancellationToken cancellationToken)
        {
            // await CreateTopicMaybe(topic, 1, 1, _clientConfig);

            using var producer = _producerBuilder.Build();

            var val = JObject.FromObject(message).ToString(Formatting.None);

            await producer.ProduceAsync(topic,
                                        new Message<string, string> {Key = null, Value = val},
                                        CancellationToken.None);

            producer.Flush(TimeSpan.FromSeconds(10));
        }

        public IEnumerable<JObject> Consume(string topic, TimeSpan? consumeTimeout = null)
        {
            //TODO to appsettings config
            var consumerConfig = new ConsumerConfig(_clientConfig);
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
                    if (cr is not null)
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