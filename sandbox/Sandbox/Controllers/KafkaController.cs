using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sandbox.Services;

namespace Sandbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class KafkaController : ControllerBase
    {
        private readonly ILogger<KafkaController> _logger;
        private readonly IKafkaClientService _kafkaClientService;

        public KafkaController(ILogger<KafkaController> logger, IKafkaClientService kafkaClientService)
        {
            _logger = logger;
            _kafkaClientService = kafkaClientService;
        }

        [HttpPost, Route("produce")]
        public Task ProduceToKafka([FromQuery] string value, [FromQuery] string topic, CancellationToken cancellationToken)
        {
            return _kafkaClientService.Produce(topic, new { Text = value }, cancellationToken);
        }
    }
}