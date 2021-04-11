using System.Net;
using Crude.FunctionalTesting.Core.Dependencies;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;

namespace Crude.FunctionalTesting.Dependency.Http
{
    public class HttpMockDependency : IDependency
    {
        private readonly HttpMockDependencyContext _context;
        private readonly MountebankClient _client;

        public HttpMockDependency(HttpMockDependencyContext context, MountebankClient mountebankClient)
        {
            _context = context;
            _client = mountebankClient;
        }

        public HttpStub AddGetMock(string path, object response)
        {
            return CreateStub(Method.Get, path, response);
        }

        public HttpStub AddPostMock(string path, object response)
        {
            return CreateStub(Method.Post, path, response);
        }

        private HttpStub CreateStub(Method method, string path, object response)
        {
            var (_, port) = _context.GetHostAndPort();
            var imposter = _client.CreateHttpImposter(port, $"{path}-mock", recordRequests: true);

            var stub = imposter.AddStub()
                .OnPathAndMethodEqual(path, method)
                .ReturnsJson(HttpStatusCode.OK, response);

            _client.Submit(imposter);

            return stub;
        }
    }
}