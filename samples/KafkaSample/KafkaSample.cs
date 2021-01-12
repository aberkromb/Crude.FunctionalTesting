using System;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Crude.FunctionalTesting.Core;
using Crude.FunctionalTesting.Core.Dependencies;
using Crude.FunctionalTesting.Dependency.Kafka;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace KafkaSample
{
    public class KafkaSample
    {
        private readonly IDependencyManager _dependencyManager;

        private KafkaDependency Kafka => _dependencyManager.GetDependency<KafkaDependency>();

        public KafkaSample()
        {
            var runningDependencies = new DependenciesBuilder()
                                      .AddDependency(new KafkaDependencyBuilder()
                                                     .AddConfig(KafkaDependencyConfig.Default)
                                                     .AddConfigureServices(_ => { }))
                                      .Start();

            _dependencyManager = runningDependencies.ConfigureServices(null, null);
        }


        [Fact]
        public async Task Kafka_ProduceConsume_WorkCorrect()
        {
            // arrange


            // act
            await Kafka.Produce("test", new {Text = "test text"});
            var messages = Kafka.Consume("test");


            // assert
            messages.Should().Contain(o => o["Text"].Value<string>() == "test text");
        }
    }
}