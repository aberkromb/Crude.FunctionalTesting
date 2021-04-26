using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Crude.FunctionalTesting.Dependency.RabbitMQ.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ
{
    public class RabbitMqDependency : IDependency
    {
        private readonly RabbitMqRunningDependencyContext _context;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private RabbitMqApiClient _apiClient;

        public RabbitMqDependency(RabbitMqRunningDependencyContext context)
        {
            _context = context;

            var config = (RabbitMqDependencyConfig) context.DependencyConfig;

            var factory = new ConnectionFactory()
            {
                HostName = context.DependencyConfig.DockerHost, Port = (int) config.ExposePort
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _apiClient = new RabbitMqApiClient(context);
        }

        public async Task<List<JObject>> Consume(string queueName)
        {
            var messages = await _apiClient.GetMessagesAsync(new GetMessagesParameters { QueueName = queueName });

            return messages.Select(m => JObject.Parse(m.Payload)).ToList();
        }

        public async Task<IReadOnlyCollection<T>> Consume<T>(string queueName)
        {
            var jObjectMessages = await Consume(queueName);

            var result = jObjectMessages.Select(msg => msg.ToObject<T>()).ToList();

            return result;
        }

        public void Publish(
            QueueParameters queueParameters,
            ExchangeParameters exchangeParameters,
            PublishParameters parameters)
        {
            SetupTopology(queueParameters, exchangeParameters, parameters);

            var messageString = MessageToString(parameters.Message);

            var body = Encoding.UTF8.GetBytes(messageString);

            _channel.BasicPublish(
                exchange: exchangeParameters.Name,
                routingKey: parameters.RoutingKey,
                basicProperties: null,
                body: body);
        }

        private void SetupTopology(QueueParameters queueParameters,
            ExchangeParameters exchangeParameters,
            PublishParameters parameters)
        {
            _channel.ExchangeDeclare(
                exchange: exchangeParameters.Name,
                type: exchangeParameters.Type,
                durable: exchangeParameters.Durable,
                autoDelete: exchangeParameters.AutoDelete,
                arguments: exchangeParameters.Arguments
            );

            _channel.QueueDeclare(
                queue: queueParameters.Name,
                durable: queueParameters.Durable,
                exclusive: queueParameters.Exclusive,
                autoDelete: queueParameters.AutoDelete,
                arguments: queueParameters.Arguments);

            _channel.QueueBind(queueParameters.Name,
                exchangeParameters.Name,
                parameters.RoutingKey,
                parameters.Arguments);
        }


        private string MessageToString(object message)
        {
            return message switch
            {
                string str => str,
                _ => JsonConvert.SerializeObject(message)
            };
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }

    public class PublishParameters
    {
        public object Message { get; set; }
        public string RoutingKey { get; set; }
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();
    }

    public class ExchangeParameters
    {
        public string Name { get; set; }
        public string Type { get; set; } = "topic";
        public bool Durable { get; set; } = true;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        public static ExchangeParameters DefaultWithName(string name) => new ExchangeParameters { Name = name };
    }

    public class QueueParameters
    {
        public string Name { get; set; }
        public bool Durable { get; set; } = true;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

        public static QueueParameters DefaultWithName(string name) => new QueueParameters { Name = name };
    }
}