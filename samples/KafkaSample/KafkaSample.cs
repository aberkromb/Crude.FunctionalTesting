using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Crude.FunctionalTesting.Core;
using Crude.FunctionalTesting.Core.Dependencies;
using Crude.FunctionalTesting.Dependency.Kafka;
using Crude.FunctionalTesting.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Sandbox;
using Xunit;

namespace KafkaSample
{
    public class KafkaSample : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactoryBuilder<Startup> _testServer;

        private KafkaDependency Kafka => _testServer.DependencyManager.GetDependency<KafkaDependency>();

        public KafkaSample(WebApplicationFactoryBuilder<Startup> factory)
        {
            _testServer = factory.AddDependenciesBuilder(new DependenciesBuilder()
                                                             .AddDependency(new KafkaDependencyBuilder()
                                                                            .AddConfig(KafkaDependencyConfig.Default)
                                                                            .AddConfigureServices(context =>
                                                                            {
                                                                                var kafkaContext = (KafkaRunningDependencyContext) context;
                                                                                context.Services.PostConfigure<ClientConfig>(options =>
                                                                                    options.BootstrapServers = kafkaContext.GetClientConfig().BootstrapServers);
                                                                            }))
                                                        );
        }


        [Fact]
        public async Task Kafka_ProduceConsume_WorkCorrect()
        {
            // arrange
            var httpClient = _testServer.CreateClient();
            var topicName = "TestTopic";
            var value = "Test Value";
            
            // act
            await Kafka.Produce("test", new { Text = "test text" });
            var x=          await httpClient.PostAsync($"kafka/produce?topic={topicName}&value={value}", null);
            var messages = Kafka.Consume(topicName);


            // assert
            messages.Should().Contain(obj => obj["Text"].Value<string>() == value);
        }
    }
}