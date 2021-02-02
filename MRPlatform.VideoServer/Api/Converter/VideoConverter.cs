using System.IO.Abstractions;
using System.Threading.Tasks;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Services;
using Microsoft.Extensions.Options;

namespace MRPlatform.VideoServer.Api.Converter
{
    public interface IVideoConverter
    {
        Task ConvertAsync(string inputFilePath, string outputFilePath);
    }

    public class VideoConverter : IVideoConverter
    {
        private readonly MediaToolkitOptions _options;

        private static readonly IFileSystem _fileSystem = new FileSystem();

        public VideoConverter(IOptions<MediaToolkitOptions> options)
        {
            _options = options.Value;
        }

        public Task ConvertAsync(string inputFilePath, string outputFilePath)
        {
            var inputFile = CreateMediaFile(inputFilePath);
            var outputFile = CreateMediaFile(outputFilePath);

            return Task.Run(() => Convert(inputFile, outputFile) );
        }

        private void Convert(MediaFile inputFile, MediaFile outputFile)
        {
            using var engine = new Engine(_options, _fileSystem);
            engine.Convert(inputFile, outputFile);
        }

        private static MediaFile CreateMediaFile(string inputFilePath)
        {
            return new MediaFile { Filename = inputFilePath };
        }
    }
}