using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Dapper;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    internal class OracleRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly IContainerService _container;
        private readonly OracleDependencyConfig _config;
        private OracleRunningDependencyContext _context;

        public OracleRunningDependency(Action<IRunningDependencyContext> configureServices,
            IContainerService container,
            OracleDependencyConfig config)
        {
            _configureServices = configureServices;
            _container = container;
            _config = config;
        }

        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new OracleRunningDependencyContext(_config, _container, configuration, services);
            _configureServices(_context);
        }

        public async Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken)
        {
            await WaitForDbStart();

            await CreateUser();

            return new OracleDependency(_context);
        }

        private async Task WaitForDbStart()
        {
            var shouldWait = true;

            while (shouldWait)
            {
                try
                {
                    await Execute("SELECT 1 FROM DUAL");
                    shouldWait = false;
                }
                catch (OracleException e) when (e.Message.StartsWith("ORA-01033") 
                                                || e.Message.StartsWith("ORA-12537")
                                                || e.Message.StartsWith("ORA-12528")
                                                || e.Message.StartsWith("ORA-12514"))
                {
                    await Task.Delay(500);
                    // ignore -> initialization in progress
                }
            }
        }

        private async Task CreateUser()
        {
            await Execute("alter session set \"_ORACLE_SCRIPT\"=true");
            await ExecuteWithIgnore($"CREATE USER FUNCTIONALTESTING IDENTIFIED BY {_config.Password}",
                new[] {"ORA-01920"});
            await Execute("GRANT CONNECT TO FUNCTIONALTESTING");
            await Execute("GRANT CONNECT, RESOURCE, DBA TO FUNCTIONALTESTING");
            await Execute("GRANT CREATE SESSION TO FUNCTIONALTESTING");
            await Execute("GRANT UNLIMITED TABLESPACE TO FUNCTIONALTESTING");
        }

        private async Task ExecuteWithIgnore(string sql, string[] ignoreExceptions = null)
        {
            await using var connection = CreateConnection();

            try
            {
                await connection.QueryAsync(sql);
            }
            catch (OracleException e) when (ignoreExceptions?.All(ignored => e.Message.StartsWith(ignored)) ?? false)
            {
                // ignore
            }
        }

        private async Task Execute(string sql)
        {
            await using var connection = CreateConnection();

            await connection.QueryAsync(sql);
        }

        private OracleConnection CreateConnection()
        {
            var connectionString =
                $"Data Source={_context.GetDependencyAddress()}:{_config.ExposePort}/ORCLCDB; User ID=SYSTEM; Password={_config.Password}";

            var connection = new OracleConnection(connectionString);

            return connection;
        }

        // private static async Task<IEnumerable<string>> GetAllTablesName(DbConnection connection, CancellationToken cancellationToken)
        // {
        //     var result = new List<string>();
        //
        //     await using var cmd = GetAllTablesCommand(connection);
        //     await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
        //     while (await reader.ReadAsync(cancellationToken))
        //         result.Add($"{reader.GetString(0)}.{reader.GetString(1)}");
        //
        //     return result;
        // }
        //
        // private static DbCommand GetAllTablesCommand(DbConnection connection)
        // {
        //     return new DbCommand(
        //         @"SELECT t.table_schema, table_name
        //             FROM information_schema.tables as t
        //             join (select s.nspname as table_schema,
        //             s.oid     as schema_id,
        //             u.usename as owner
        //             from pg_catalog.pg_namespace s
        //                 join pg_catalog.pg_user u on u.usesysid = s.nspowner
        //             where nspname not in ('information_schema', 'pg_catalog'/*, 'public'*/)
        //             and nspname not like 'pg_toast%'
        //             and nspname not like 'pg_temp_%'
        //             order by table_schema
        //                 ) as s
        //                 on s.table_schema = t.table_schema
        //             WHERE t.table_type = 'BASE TABLE'"
        //         , connection);
        // }
        //
        // private static async Task TruncateTables(NpgsqlConnection connection,
        //                                          IEnumerable<string> tablesNames,
        //                                          CancellationToken cancellationToken)
        // {
        //     await using var cmd = GetTruncateTablesCommand(tablesNames, connection);
        //     await cmd.ExecuteNonQueryAsync(cancellationToken);
        // }
        //
        // private static NpgsqlCommand GetTruncateTablesCommand(IEnumerable<string> tablesNames, NpgsqlConnection connection)
        // {
        //     return new NpgsqlCommand($"truncate {string.Join(',', tablesNames)} cascade", connection);
        // }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}