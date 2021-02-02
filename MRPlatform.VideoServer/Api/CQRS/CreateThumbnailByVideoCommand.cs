using System;
using System.Threading.Tasks;
using MediaToolkit.Services;
using MediaToolkit.Tasks;

namespace MRPlatform.VideoServer.Api.CQRS
{
    public interface ICreateThumbnailByVideoCommand
    {
        Task ExecuteAsync(string videoPath, string thumbnailPath);
    }

    public class CreateThumbnailByVideoCommand : ICreateThumbnailByVideoCommand
    {
        private static readonly TimeSpan SeekSpan = TimeSpan.FromSeconds(1);
        private readonly IMediaToolkitService _mediaToolkitService;

        public CreateThumbnailByVideoCommand(IMediaToolkitService mediaToolkitService)
        {
            _mediaToolkitService = mediaToolkitService;
        }

        public Task ExecuteAsync(string videoPath, string thumbnailPath)
        {
            var saveThumbnailTask = new FfTaskSaveThumbnail(videoPath, thumbnailPath, SeekSpan);
            return _mediaToolkitService.ExecuteAsync(saveThumbnailTask);
        }
    }
}
