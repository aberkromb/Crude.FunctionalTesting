using System.Net;
using MbDotNet;
using MbDotNet.Enums;
using MbDotNet.Models.Stubs;

namespace Crude.FunctionalTesting.Dependencies.Http
{
    public class HttpMockDependency : IDependency
    {
        private readonly HttpMockDependencyContext _context;
        private readonly MountebankClient _mountebankClient;
        private MountebankClient _client;

        public HttpMockDependency(HttpMockDependencyContext context, MountebankClient mountebankClient)
        {
            _context = context;
            _mountebankClient = mountebankClient;
            _client = new MountebankClient();
        }
        
        public HttpStub AddGetMock(string path, object response)
        {
            var (_, port) = _context.GetHostAndPort();
            var imposter = _client.CreateHttpImposter(port, $"{path}-mock-", recordRequests: true); 
            
            var stub = imposter.AddStub()
                .OnPathAndMethodEqual(path, Method.Get)
                .ReturnsJson(HttpStatusCode.OK, response);
            
            _client.Submit(imposter);
            
            return stub;
        }
    }
}