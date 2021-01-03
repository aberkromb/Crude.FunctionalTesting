using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace Crude.FunctionalTesting.Dependencies.Postgres
{
    /// <summary>
    ///     Сервис для работы с postgres
    /// </summary>
    public class PostgresDependency : IDependency
    {
        private readonly PostgresRunningDependencyContext _context;

        public PostgresDependency(PostgresRunningDependencyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<dynamic>> Execute(string sql, object parameters = null)
        {
            await using var connection = CreateConnection();
            
            var result = await connection.QueryAsync(sql, parameters);

            return result;
        }

        public async Task<dynamic> ExecuteFirst(string sql, object parameters = null)
        {
            return (await Execute(sql, parameters)).First();
        }

        private NpgsqlConnection CreateConnection()
        {
            var connString = _context.ConnectionString;
            return new NpgsqlConnection(connString);
        }
    }
}