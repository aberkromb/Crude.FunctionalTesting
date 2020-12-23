using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sandbox.DataAccess;
using Sandbox.DataAccess.DataBase;

namespace Sandbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiTestController : ControllerBase
    {
        private readonly ILogger<ApiTestController> _logger;
        private readonly IDataAccess _dataAccess;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiTestController(ILogger<ApiTestController> logger, IDataAccess dataAccess,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dataAccess = dataAccess;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost, Route("postgres")]
        public async Task<string> InsertToPostgres([FromQuery] string toInsert, CancellationToken cancellationToken)
        {
            var inserted = await _dataAccess.Add(new Strings {String = toInsert}, cancellationToken);
            var value = (await _dataAccess.Get(inserted.Id, cancellationToken)).String;
            return value;
        }

        [HttpGet, Route("postgres")]
        public async Task<string> GetFromPostgres([FromQuery] int id, CancellationToken cancellationToken)
        {
            var value = (await _dataAccess.Get(id, cancellationToken)).String;
            return value;
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