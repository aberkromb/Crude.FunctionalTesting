using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sandbox.DataAccess;
using Sandbox.DataAccess.DataBase;

namespace Sandbox.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostgresController : ControllerBase
    {
        private readonly ILogger<PostgresController> _logger;
        private readonly IDataAccess _dataAccess;

        public PostgresController(ILogger<PostgresController> logger,
                                  IDataAccess dataAccess)
        {
            _logger = logger;
            _dataAccess = dataAccess;
        }

        [HttpPost, Route("postgres")]
        public async Task<string> InsertToPostgres([FromQuery] string toInsert, CancellationToken cancellationToken)
        {
            var inserted = await _dataAccess.Add(new Strings { String = toInsert }, cancellationToken);
            var value = (await _dataAccess.Get(inserted.Id, cancellationToken)).String;
            return value;
        }

        [HttpGet, Route("postgres")]
        public async Task<string> GetFromPostgres([FromQuery] int id, CancellationToken cancellationToken)
        {
            var value = (await _dataAccess.Get(id, cancellationToken)).String;
            return value;
        }
    }
}