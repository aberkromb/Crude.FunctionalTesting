using System;
using System.Threading.Tasks;
using Crude.FunctionalTesting;
using Crude.FunctionalTesting.Dependencies.Postgres;
using Crude.FunctionalTesting.TestServer;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Sandbox;
using Sandbox.DataAccess;
using Xunit;

namespace Crude.FunctionalTestingFunctionalTests
{
    public class PostgresTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _testServer;

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
                                    options.ConnectionString = postgresContext.GetConnectionString);
                            })));
        }


        [Fact]
        public async Task InsertToPostgres_Return_ExpectedValue()
        {
            // arrange
            var client = _testServer.CreateClient();
            var insertData = Guid.NewGuid().ToString();
            var expected = insertData;

            // act 
            var response = await client.GetAsync($"apitest/postgres?toInsert={insertData}");
            var result = await response.Content.ReadAsStringAsync();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}