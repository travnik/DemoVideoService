using System;
using System.IO;
using FileProvider.Api;
using MRPlatform.VideoServer.Common;

namespace MRPlatform.VideoServer.CQRS
{
    public interface IGetFileByIdQuery
    {
        Stream Execute(string fileId, string extension);
    }

    public class GetFileByIdQuery : IGetFileByIdQuery
    {
        private readonly IFileRepository _fileRepository;

        public GetFileByIdQuery(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public Stream Execute(string fileId, string extension)
        {
            var filePath = FileNameHelper.GetFullFilePathByFileId(fileId, extension);

            return _fileRepository.IsExists(filePath) 
                ? _fileRepository.GetStream(filePath) 
                : null;
        }

        
    }
}
