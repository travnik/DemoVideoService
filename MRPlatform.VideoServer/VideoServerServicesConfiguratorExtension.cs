using MediaStorage;
using MediaToolkit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MRPlatform.VideoServer.Api.Converter;
using MRPlatform.VideoServer.Api.CQRS;
using MRPlatform.VideoServer.CQRS;

namespace MRPlatform.VideoServer
{
    public static class VideoServerServicesConfiguratorExtension
    {
        public static void ConfigureVideoServer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFileUploadCommand, FileUploadCommand>();
            services.AddScoped<IGetFileByIdQuery, GetFileByIdQuery>();
            services.AddScoped<IGetVideoMpeg4Command, GetVideoMpeg4Command>();
            services.AddScoped<IGetThumbnailJpegCommand, GetThumbnailJpegCommand>();
            services.AddScoped<IRemoveFilesByFileIdCommand, RemoveFilesByFileIdCommand>();

            services.AddMediaToolkit(configuration);
            ConfigureApi(services, configuration);
            services.ConfigureMedia(configuration);
        }

        private static void ConfigureApi(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IVideoConverter, VideoConverter>();
            services.AddScoped<ICreateThumbnailByVideoCommand, CreateThumbnailByVideoCommand>();
        }
    }
}
