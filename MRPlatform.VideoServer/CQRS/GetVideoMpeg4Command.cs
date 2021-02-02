using System;
using System.Threading.Tasks;
using FileProvider.Api;
using MRPlatform.VideoServer.Api.Converter;
using MRPlatform.VideoServer.Common.FileProvider;

namespace MRPlatform.VideoServer.CQRS
{
    public interface IGetVideoMpeg4Command
    {
        Task<MediaFile> ExecuteAsync(string fileId);
    }

    public class GetVideoMpeg4Command : MediaFileProvider, IGetVideoMpeg4Command
    {
        private readonly IVideoConverter _videoConverter;

        public GetVideoMpeg4Command(IGetFileByIdQuery fileQuery, 
            IFolderRepository folderRepository, 
            IPathWrapper pathWrapper, 
            IVideoConverter videoConverter) : base(fileQuery, folderRepository, pathWrapper)
        {
            _videoConverter = videoConverter;
        }

        public Task<MediaFile> ExecuteAsync(string fileId)
        {
            return GetOrCreateFileByFileIdAsync(fileId, FileExtensionFormat.Mp4);
        }

        protected override Task CreateFileAsync(string sourcefilePath, string destFilePath)
        {
            return _videoConverter.ConvertAsync(sourcefilePath, destFilePath);
        }
    }
}
