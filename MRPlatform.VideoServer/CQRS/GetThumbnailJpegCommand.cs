using System;
using System.Threading.Tasks;
using FileProvider.Api;
using MRPlatform.VideoServer.Api.CQRS;
using MRPlatform.VideoServer.Common.FileProvider;

namespace MRPlatform.VideoServer.CQRS
{
    public interface IGetThumbnailJpegCommand
    {
        Task<MediaFile> ExecuteAsync(string fileId);
    }

    public class GetThumbnailJpegCommand : MediaFileProvider, IGetThumbnailJpegCommand
    {
        private readonly ICreateThumbnailByVideoCommand _createThumbnailByVideoCommand;

        public GetThumbnailJpegCommand(IGetFileByIdQuery fileQuery, 
            IFolderRepository folderRepository, 
            IPathWrapper pathWrapper, 
            ICreateThumbnailByVideoCommand thumbnailByVideoCommand) : base(fileQuery, folderRepository, pathWrapper)
        {
            _createThumbnailByVideoCommand = thumbnailByVideoCommand;
        }

        public Task<MediaFile> ExecuteAsync(string fileId)
        {
            return GetOrCreateFileByFileIdAsync(fileId, FileExtensionFormat.Jpeg);
        }

        protected override Task CreateFileAsync(string sourcefilePath, string destFilePath)
        {
            return _createThumbnailByVideoCommand.ExecuteAsync(sourcefilePath, destFilePath);
        }
    }
}
