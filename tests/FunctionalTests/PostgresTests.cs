using System;
using System.Threading.Tasks;
using Crude.FunctionalTesting;
using Crude.FunctionalTesting.Dependencies.Postgres;
using Crude.FunctionalTesting.TestServer;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sandbox;
using Sandbox.DataAccess;
using Xunit;

namespace FunctionalTests
{
    public class PostgresTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactoryBuilder<Startup> _testServer;
        private PostgresDependency Postgres => _testServer.DependencyManager.GetDependency<PostgresDependency>();

        public PostgresTests(WebApplicationFactoryBuilder<Startup> factory)
        {
            _testServer = factory.AddDependenciesBuilder(
                new DependenciesBuilder()
                    .AddDependency(
                        new PostgresDependencyBuilder()
                            .AddConfig(PostgresDependencyConfig.Default)
                            .AddConfigureServices(context =>
                            {
                                var postgresContext = (PostgresRunningDependencyContext) context;
                                context.Services.PostConfigure<PostgresOptions>(options =>
                                    options.ConnectionString = postgresContext.ConnectionString);
                            })));
        }

        [Fact]
        public async Task InsertToPostgres_Return_ExpectedValue()
        {
            // arrange
            var httpClient = _testServer.CreateClient();
            var insertData = Guid.NewGuid().ToString();

            // act 
            var response = await httpClient.PostAsync($"apitest/postgres?toInsert={insertData}", null);
            var result = await response.Content.ReadAsStringAsync();
            var expected = (await Postgres.ExecuteFirst("SELECT * FROM strings")).String;

            // assert
            result.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public async Task GetById_Return_ExpectedValue()
        {
            // arrange
            var httpClient = _testServer.CreateClient();
            var insertData = Guid.NewGuid().ToString();
            var expected = insertData;
            await Postgres.Execute($"INSERT INTO strings (\"Id\", \"String\") values (10, '{insertData}')");
                
            // act 
            var response = await httpClient.GetAsync("apitest/postgres?id=10");
            var result = await response.Content.ReadAsStringAsync();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}