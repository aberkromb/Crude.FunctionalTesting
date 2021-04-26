using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Crude.FunctionalTesting.Dependency.RabbitMQ.Internal
{
    internal class RabbitMqApiClient
    {
        private readonly RabbitMqRunningDependencyContext _context;
        private static HttpClient _httpClient = new HttpClient();
        private readonly RabbitMqDependencyConfig _config;

        public RabbitMqApiClient(RabbitMqRunningDependencyContext context)
        {
            _context = context;
            _config = (RabbitMqDependencyConfig) context.DependencyConfig;
        }

        public async Task<RabbitMqMessage[]> GetMessagesAsync(GetMessagesParameters parameters)
        {
            // TODO сделать поиск по регулярке
            
            var auth = "guest:guest";

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(auth)));

            var requestJson = JsonConvert.SerializeObject(new
            {
                vhost = parameters.Vhost,
                name = parameters.QueueName,
                truncate = 50000,
                ackmode = "ack_requeue_true",
                encoding = "auto",
                count = parameters.Count
            });

            var response = await _httpClient.PostAsync(
                new Uri(
                    $"http://{_context.GetDependencyAddress()}:{_config.UiPort}/api/queues/{HttpUtility.UrlEncode(parameters.Vhost)}/{parameters.QueueName}/get"),
                new StringContent(requestJson));

            var json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<RabbitMqMessage[]>(
                json,
                new JsonSerializerSettings
                {
                    ContractResolver =
                        new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                });
        }
    }

    public class RabbitMqMessage
    {
        public int PayloadBytes { get; set; }
        public bool Redelivered { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public int MessageCount { get; set; }
        public KeyValuePair<string, object> Properties { get; set; }
        public string Payload { get; set; }
        public string PayloadEncoding { get; set; }
    }


    public class GetMessagesParameters
    {
        public string Vhost { get; set; } = "/";

        public string QueueName { get; set; }

        public int Count { get; set; } = 100;
    }
}