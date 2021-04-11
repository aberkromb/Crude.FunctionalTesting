using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Dapper;
using Oracle.ManagedDataAccess.Client;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    /// <summary>
    ///     Сервис для работы с postgres
    /// </summary>
    public class OracleDependency : IDependency
    {
        private readonly OracleRunningDependencyContext _context;

        public OracleDependency(OracleRunningDependencyContext context)
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

        private OracleConnection CreateConnection()
        {
            var connectionString = _context.ConnectionString;
            
            var connection = new OracleConnection(connectionString);
            
            return connection;
        }

        public void Dispose()
        {
        }
    }
}