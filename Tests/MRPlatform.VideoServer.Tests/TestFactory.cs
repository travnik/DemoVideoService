using System.Collections.Generic;
using MediaToolkit;
using MediaToolkit.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MRPlatform.VideoServer.Api.Converter;
using MRPlatform.VideoServer.Api.CQRS;

namespace MRPlatform.VideoServer.Tests
{
    public static class TestFactory
    {
        public const string InputFilePath = "BigBunny.m4v";
        public const string InputFilePathMov = "test.mov";
        public const string OutputFilePath = "result.mp4";
        public const string OutputJpegFilePath = "result.jpg";
        public const string FfmpegFilePath = "C:\\tmp\\ffmpeg\\bin\\ffmpeg.exe";
        public const string FfProbeFilePath = "C:\\tmp\\ffmpeg\\bin\\ffprobe.exe";

        public static IVideoConverter GetVideoConverter()
        {
            var converter = new VideoConverter(Options.Create(new MediaToolkitOptions()
            {
                FfMpegPath = FfmpegFilePath,
                FfProbePath = FfProbeFilePath
            }));

            return converter;
        }

        public static ICreateThumbnailByVideoCommand GetCreateThumbnailByVideoCommand()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"MediaToolkitOptions:FfMpegPath", FfmpegFilePath},
                    {"MediaToolkitOptions:FfProbePath", FfProbeFilePath}
                });
            var configuration = builder.Build();

            var serviceProvider = new ServiceCollection()
                .AddMediaToolkit(configuration)
                .BuildServiceProvider();

            var service = serviceProvider.GetService<IMediaToolkitService>();
            return new CreateThumbnailByVideoCommand(service);
        }
    }
}
