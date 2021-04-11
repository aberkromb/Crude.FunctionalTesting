using System.Threading.Tasks;
using Crude.FunctionalTesting.Core;
using Crude.FunctionalTesting.Dependency.RabbitMQ;
using Crude.FunctionalTesting.TestServer;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Sandbox;
using Xunit;

namespace RabbitMqSample
{
    public class RabbitMqSample : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactoryBuilder<Startup> _testServer;

        private RabbitMqDependency RabbitMq => _testServer.DependencyManager.GetDependency<RabbitMqDependency>();

        public RabbitMqSample(WebApplicationFactoryBuilder<Startup> factory)
        {
            _testServer = factory.AddDependenciesBuilder(new DependenciesBuilder()
                .AddDependency(new RabbitMqDependencyBuilder()
                    .AddConfig(RabbitMqDependencyConfig.Default)
                    .AddConfigureServices(context =>
                    {
                        var rabbitmqContext = (RabbitMqRunningDependencyContext) context;
                        // context.Services.PostConfigure<ClientConfig>(options =>
                        //     options.BootstrapServers = rabbitmqContext.GetClientConfig()
                        //         .BootstrapServers);
                    }))
            );
        }


        [Fact]
        public async Task RabbitMq_PublishConsume_WorkCorrect()
        {
            // arrange
            var httpClient = _testServer.CreateClient();
            var queueParameters = QueueParameters.DefaultWithName("test-q");
            var exchangeParameters = ExchangeParameters.DefaultWithName("test-ex");
            var publishParameters = new PublishParameters { Message = new { Text = "test message"}, RoutingKey = "test-rk" };

            // act
            RabbitMq.Publish(queueParameters, exchangeParameters, publishParameters);
            await Task.Delay(100);
            var messages = RabbitMq.Consume(exchangeParameters.Name, queueParameters.Name);

            // assert
            messages.Should().Contain(jObject => jObject["Text"].Value<string>() == "test message");
        }

        // [Fact]
        public async Task RabbitMq_ProduceConsume_WorkCorrect()
        {
            // // arrange
            // var httpClient = _testServer.CreateClient();
            // var topicName = "TestTopic";
            // var value = "Test Value";
            //
            // // act
            // await RabbitMq.Produce(topicName, new { Text = "test text" });
            // var x = await httpClient.PostAsync($"kafka/produce?topic={topicName}&value={value}", null);
            // var messages = RabbitMq.Consume(topicName);
            //
            //
            // // assert
            // messages.Should().Contain(obj => obj["Text"].Value<string>() == value);
        }
    }
}