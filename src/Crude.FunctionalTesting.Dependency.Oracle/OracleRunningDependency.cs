using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core.Dependencies;
using Dapper;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oracle.ManagedDataAccess.Client;
using Respawn;

namespace Crude.FunctionalTesting.Dependency.Oracle
{
    internal class OracleRunningDependency : IRunningDependency
    {
        private readonly Action<IRunningDependencyContext> _configureServices;
        private readonly IContainerService _container;
        private readonly OracleDependencyConfig _config;
        private OracleRunningDependencyContext _context;

        private static Checkpoint _checkpoint = new Checkpoint
        {
            SchemasToInclude = new []
            {
                "FUNCTIONALTESTING"
            },
            DbAdapter = DbAdapter.Oracle
        };

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
            await ResetDb();

            return new OracleDependency(_context);
        }

        private async Task ResetDb()
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();
            await _checkpoint.Reset(connection);
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
                catch (OracleException e) when (e.Message.Contains("ORA-01033", StringComparison.InvariantCultureIgnoreCase)
                                                || e.Message.Contains("ORA-12537", StringComparison.InvariantCultureIgnoreCase)
                                                || e.Message.Contains("ORA-12528", StringComparison.InvariantCultureIgnoreCase)
                                                || e.Message.Contains("ORA-12514", StringComparison.InvariantCultureIgnoreCase))
                {
                    await Task.Delay(500);
                    // ignore -> initialization in progress
                }
            }
        }

        private async Task CreateUser()
        {
            await Execute("alter session set \"_ORACLE_SCRIPT\"=true");
            await ExecuteWithIgnore(
                $"CREATE USER FUNCTIONALTESTING IDENTIFIED BY {_config.Password}",
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
            catch (OracleException e) when (
                ignoreExceptions?.Any(ignored => e.Message.Contains(ignored, StringComparison.InvariantCultureIgnoreCase))
                ?? false)
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

        public void Dispose()
        {
            _container?.Dispose();
        }
    }
}