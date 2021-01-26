using System;
using System.Threading.Tasks;
using Crude.FunctionalTesting.Core;
using Crude.FunctionalTesting.Core.Dependencies;
using Crude.FunctionalTesting.Dependency.Oracle;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using OracleDependency = Crude.FunctionalTesting.Dependency.Oracle.OracleDependency;
using FluentAssertions;

namespace OracleBaseSample
{
    public class UnitTest1
    {
        private readonly IDependencyManager _dependencyManager;

        private OracleDependency Oracle => _dependencyManager.GetDependency<OracleDependency>();

        public UnitTest1()
        {
            _dependencyManager = new DependenciesBuilder()
                .AddDependency(new OracleDependencyBuilder()
                    .AddConfig(OracleDependencyConfig.Default)
                    .AddConfigureServices(_ => { }))
                .Start().ConfigureServices(null, null);
        }

        [Fact]
        public async Task Select_ShouldReturnValue()
        {
            // arrange
            await EnsureTableCreated();
            await Oracle.Execute("INSERT INTO \"SYSTEM\".CLIENTS (ID, CLIENT_TYPE) VALUES (9182988, 1)");

            // act 
            var expected = (decimal)(await Oracle.ExecuteFirst("SELECT * FROM \"SYSTEM\".CLIENTS")).ID;

            // assert
            expected.Should().Be(9182988);
        }

        private async Task EnsureTableCreated()
        {
            try
            {
                await Oracle.Execute(
                    "create table \"SYSTEM\".CLIENTS (ID NUMBER(38) not null primary key, CLIENT_TYPE NUMBER)");
            }
            catch (OracleException e) when(e.Message.StartsWith("ORA-00955", StringComparison.OrdinalIgnoreCase))
            {
                // ignore - table already exists
            }
        }
    }
}