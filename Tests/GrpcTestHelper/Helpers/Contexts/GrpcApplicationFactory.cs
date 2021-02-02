using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace GrpcTestHelper.Helpers.Contexts
{
    public interface IApplicationFactory
    {
        IServiceProvider GetServiceProvider();
    }

    public class GrpcApplicationFactory<TEntryPoint> : BaseWebApplicationFactory<TEntryPoint>, IApplicationFactory
        where TEntryPoint : class
    {
        public Action<IServiceCollection> ConfigureServicesAction { get; set; }

        public GrpcApplicationFactory(IEnumerable<KeyValuePair<string, string>> properties = null)
        {
            _properties = properties;
            JsonFiles.Add(TestConst.AppSettingNameDefault);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureTestServices(o => ConfigureServicesAction?.Invoke(o));
        }

        public IServiceProvider GetServiceProvider()
        {
            return Server.Services.CreateScope().ServiceProvider;
        }

        public GrpcChannel CreateGrpcChannel()
        {
            var client = CreateDefaultClient(new ResponseVersionHandler());
            return GrpcChannel.ForAddress(client.BaseAddress, new GrpcChannelOptions
            {
                HttpClient = client,
                Credentials = ChannelCredentials.Insecure
            });
        }

        public TService GetRequiredService<TService>()
        {
            return GetServiceProvider().GetRequiredService<TService>();
        }

        private class ResponseVersionHandler : DelegatingHandler
        {
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                var response = await base.SendAsync(request, cancellationToken);
                response.Version = request.Version;

                return response;
            }
        }
    }
}


