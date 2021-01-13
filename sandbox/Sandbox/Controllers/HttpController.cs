using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Sandbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HttpController : ControllerBase
    {
        private readonly ILogger<ApiTestController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpController(ILogger<ApiTestController> logger,
                              IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet, Route("google")]
        public async Task<object> PingGoogle(CancellationToken cancellationToken)
        {
            var response = await _httpClientFactory
                                 .CreateClient("google")
                                 .GetAsync("http://google.com/search?q=ping", cancellationToken);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject(json);
        }
    }
}