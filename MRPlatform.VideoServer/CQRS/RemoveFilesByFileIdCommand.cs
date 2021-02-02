using System;
using FileProvider.Api;
using MRPlatform.VideoServer.Common;

namespace MRPlatform.VideoServer.CQRS
{
    public interface IRemoveFilesByFileIdCommand
    {
        void Execute(string fileId);
    }

    public class RemoveFilesByFileIdCommand : IRemoveFilesByFileIdCommand
    {
        private readonly IFolderRepository _folderRepository;

        public RemoveFilesByFileIdCommand(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;
        }

        public void Execute(string fileId)
        {
            var folderName = FileNameHelper.GetFolderPathByFileId(fileId);
            _folderRepository.Remove(folderName, true);
        }
    }
}
