using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileProvider.Api;
using MRPlatform.VideoServer.CQRS;

namespace MRPlatform.VideoServer.Common.FileProvider
{
    public abstract class MediaFileProvider
    {
        private readonly IGetFileByIdQuery _getFileQuery;
        private readonly IFolderRepository _folderRepository;
        private readonly IPathWrapper _pathWrapper;

        protected MediaFileProvider(IGetFileByIdQuery fileQuery, 
            IFolderRepository folderRepository, 
            IPathWrapper pathWrapper)
        {
            _getFileQuery = fileQuery;
            _folderRepository = folderRepository;
            _pathWrapper = pathWrapper;
        }

        protected async Task<MediaFile> GetOrCreateFileByFileIdAsync(string fileId, string extension)
        {
            var fileStream = GetFileStreamByFileIdAsync(fileId, extension) ?? 
                             await CreateFileByFileIdAsync(fileId, extension);

            return new MediaFile()
            {
                Stream = fileStream,
                FileName = FileNameHelper.GetFileName(fileId, extension)
            };
        }

        private Stream GetFileStreamByFileIdAsync(string fileId, string extension)
        {
            return _getFileQuery.Execute(fileId, extension);
        }

        private async Task<Stream> CreateFileByFileIdAsync(string fileId, string extension)
        {
            var fileInfo = GetSourceFileInfoByFileId(fileId);
            var destFileLocalPath = FileNameHelper.GetFullFilePathByFileId(fileId, extension);
            var sourceFileFullPath = fileInfo.FullName;
            var destFileFullPath = _pathWrapper.GetFullPath(destFileLocalPath);

            await CreateFileAsync(sourceFileFullPath, destFileFullPath);

            return _getFileQuery.Execute(fileId, extension);
        }

        protected abstract Task CreateFileAsync(string sourcefilePath, string destFilePath);

        private FileInfo GetSourceFileInfoByFileId(string fileId)
        {
            var folderPath = FileNameHelper.GetFolderPathByFileId(fileId);
            var folderInfo = _folderRepository.GetFolderInfo(folderPath);
            var fileInfo = folderInfo.Files
                .First(o => _pathWrapper.GetFileNameWithoutExtension(o.Name) == FileNameHelper.SourceFileName);
            return fileInfo;
        }
    }
}
