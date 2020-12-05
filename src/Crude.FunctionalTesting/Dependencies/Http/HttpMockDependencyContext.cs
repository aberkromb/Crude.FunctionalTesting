using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace Crude.FunctionalTesting.Dependencies.Http
{
    public class HttpMockDependencyContext : IRunningDependencyContext
    {
        private readonly HttpMockDependencyConfig _config;
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly IContainerService _container;

        public IConfiguration Configuration => _configuration;

        public IServiceCollection Services => _services;

        public IDependencyConfig DependencyConfig => _config;

        public HttpMockDependencyContext(HttpMockDependencyConfig config,
                                         IContainerService container,
                                         IConfiguration configuration,
                                         IServiceCollection services)
        {
            _container = container;
            _configuration = configuration;
            _services = services;
            _config = config;
        }

        public (string host, int port) GetHostAndPort() =>
            (_container.ToHostExposedEndpoint($"{_config.ExposeApiPort}/tcp").Address.ToString(),
                (int) _config.ExposeApiPort);

        public class TurnRequestToMockFilter : IHttpMessageHandlerBuilderFilter
        {
            private readonly string _host;
            private readonly int _port;

            public TurnRequestToMockFilter(string host, int port)
            {
                _host = host;
                _port = port;
            }

            public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
            {
                return (builder) =>
                {
                    next(builder);

                    builder.AdditionalHandlers.Add(new TurnRequestToMockHandler(_host, _port));
                };
            }

            private class TurnRequestToMockHandler : DelegatingHandler
            {
                private readonly string _host;
                private readonly int _port;

                public TurnRequestToMockHandler(string host, int port)
                {
                    _host = host;
                    _port = port;
                }

                private static Regex regexHost = new Regex(@":\/\/.*/", RegexOptions.Compiled);

                protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                                       CancellationToken cancellationToken)
                {
                    var oldUri = request.RequestUri.ToString();

                    var newUri = regexHost.Replace(oldUri, $"://{_host}:{_port}/");

                    request.RequestUri = new Uri(newUri);

                    return base.SendAsync(request, cancellationToken);
                }
            }
        }
    }
}