using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRPlatform.VideoServer.Grpc.Common;
using MRPlatform.VideoServer.Grpc.CQRS;

namespace MRPlatform.VideoServer.Grpc.Extensions
{
    public static class ServiceConfigurationExtension
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUploadFileStreamCommand, UploadFileStreamCommand>();
            services.AddScoped<IGetVideoStreamCommand, GetVideoStreamCommand>();
            services.AddScoped<IGetThumbnailCommand, GetThumbnailCommand>();

            services.AddScoped<IStreamChunksFlowSpliter, StreamChunksFlowSpliter>();
            services.Configure<ChunkSpliterOptions>(configuration.GetSection("ChunkSpliterOptions"));
        }
    }
}
