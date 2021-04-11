using System;
using System.Collections.Generic;
using System.Text;
using Crude.FunctionalTesting.Core.Dependencies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ
{
    public class RabbitMqDependency : IDependency
    {
        private readonly RabbitMqRunningDependencyContext _context;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        private IDictionary<(string, string), List<JObject>> ConsumedMessages =
            new Dictionary<(string, string), List<JObject>>();

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
        }

        public IEnumerable<JObject> Consume(string exchangeName, string queueName)
        {
            return ConsumedMessages[(exchangeName, queueName)];
        }

        public void Publish(
            QueueParameters queueParameters,
            ExchangeParameters exchangeParameters,
            PublishParameters parameters)
        {
            SetupTopology(queueParameters, exchangeParameters, parameters);

            var messageString = MessageToString(parameters.Message);

            var body = Encoding.UTF8.GetBytes(messageString);
            
            CreateConsumer(exchangeParameters.Name, queueParameters.Name);

            _channel.BasicPublish(
                exchange: exchangeParameters.Name,
                routingKey: parameters.RoutingKey,
                basicProperties: null,
                body: body);
        }

        private void CreateConsumer(string exchangeName, string queueName)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = JObject.Parse(Encoding.UTF8.GetString(body));
                
                if (ConsumedMessages.TryGetValue((exchangeName, queueName), out var messages))
                    messages.Add(message);
                else
                    ConsumedMessages[(exchangeName, queueName)] = new List<JObject> { message };
            };

            _channel.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);
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