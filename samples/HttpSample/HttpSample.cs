// using System.Threading.Tasks;
// using Crude.FunctionalTesting;
// using Crude.FunctionalTesting.Core;
// using Crude.FunctionalTesting.Core.Dependencies;
// using Crude.FunctionalTesting.Dependency.Http;
// using Crude.FunctionalTesting.TestServer;
// using FluentAssertions;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Http;
// using Sandbox;
// using Xunit;
//
// namespace HttpSample
// {
//     public class HttpMockTests : IClassFixture<WebApplicationFactoryBuilder<Startup>>
//     {
//         private readonly WebApplicationFactoryBuilder<Startup> _testServer;
//         private IDependencyManager DependencyManager => _testServer.DependencyManager;
//         private HttpMockDependency HttpMock => DependencyManager.GetDependency<HttpMockDependency>();
//
//         public HttpMockTests(WebApplicationFactoryBuilder<Startup> factory)
//         {
//             _testServer = factory.AddDependenciesBuilder(
//                 new DependenciesBuilder()
//                     .AddDependency(
//                         new HttpMockDependencyBuilder()
//                             .AddConfig(HttpMockDependencyConfig.Default)
//                             .AddConfigureServices(context =>
//                             {
//                                 var httpMockContext = (HttpMockDependencyContext) context;
//                                 var (host, port) = httpMockContext.GetHostAndPort();
//                                 context.Services.AddSingleton<IHttpMessageHandlerBuilderFilter>(_ =>
//                                     new HttpMockDependencyContext.TurnRequestToMockFilter(host, port));
//                             })));
//         }
//
//         [Fact]
//         public async Task ApiTestGoogle_MockSearchPath_ReturnExpectedValue()
//         {
//             // arrange
//             var client = _testServer.CreateClient();
//             HttpMock.AddGetMock("/search", new {value = "mock result"});
//
//             // act
//             var response = await client.GetAsync("http/google");
//             var result = await response.Content.ReadAsStringAsync();
//
//             // assert
//             result.Should().BeEquivalentTo("{\"value\":\"mock result\"}");
//         }
//     }
// }