using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Crude.FunctionalTesting.Dependencies.Postgres
{
    internal class PostgresRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly IContainerService _container;
        private readonly PostgresDependencyConfig _config;
        private PostgresRunningDependencyContext _context;

        public PostgresRunningDependency(Action<IRunningDependencyContext> configureServices,
                                         IContainerService container,
                                         PostgresDependencyConfig config)
        {
            _configureServices = configureServices;
            _container = container;
            _config = config;
        }

        public void ConfigureService(IConfiguration configuration, IServiceCollection services)
        {
            _context = new PostgresRunningDependencyContext(_config, _container, configuration, services);
            _configureServices(_context);
        }

        public async Task<IDependency> AfterDependencyStart(CancellationToken cancellationToken)
        {
            var connString = _context.GetConnectionString;

            await using var connection = new NpgsqlConnection(connString);
            await connection.OpenAsync(cancellationToken);
            var tablesNames = await GetAllTablesName(connection, cancellationToken);

            if (tablesNames.Any()) 
                await TruncateTables(connection, tablesNames, cancellationToken);

            return new PostgresDependency();
        }

        private static async Task<IEnumerable<string>> GetAllTablesName(NpgsqlConnection connection,
                                                                 CancellationToken cancellationToken)
        {
            var result = new List<string>();

            await using var cmd = GetAllTablesCommand(connection);
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
                result.Add($"{reader.GetString(0)}.{reader.GetString(1)}");

            return result;
        }

        private static NpgsqlCommand GetAllTablesCommand(NpgsqlConnection connection)
        {
            return new NpgsqlCommand(
                @"SELECT t.table_schema, table_name
                    FROM information_schema.tables as t
                    join (select s.nspname as table_schema,
                    s.oid     as schema_id,
                    u.usename as owner
                    from pg_catalog.pg_namespace s
                        join pg_catalog.pg_user u on u.usesysid = s.nspowner
                    where nspname not in ('information_schema', 'pg_catalog'/*, 'public'*/)
                    and nspname not like 'pg_toast%'
                    and nspname not like 'pg_temp_%'
                    order by table_schema
                        ) as s
                        on s.table_schema = t.table_schema
                    WHERE t.table_type = 'BASE TABLE'"
                , connection);
        }

        private static async Task TruncateTables(NpgsqlConnection connection,
                                                 IEnumerable<string> tablesNames,
                                                 CancellationToken cancellationToken)
        {
            await using var cmd = GetTruncateTablesCommand(tablesNames, connection);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static NpgsqlCommand GetTruncateTablesCommand(IEnumerable<string> tablesNames,
                                                              NpgsqlConnection connection)
        {
            return new NpgsqlCommand($"truncate {string.Join(',', tablesNames)} cascade", connection);
        }

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}