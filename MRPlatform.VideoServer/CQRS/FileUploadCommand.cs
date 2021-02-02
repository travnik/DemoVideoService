using System;
using System.Collections.Generic;
using System.IO;
using MediaStorage;
using MediaStorage.Models;
using MRPlatform.VideoServer.Common;

namespace MRPlatform.VideoServer.CQRS
{
    public interface IFileUploadCommand
    {
        string Execute(Stream file, string filename);
    }

    public class FileUploadCommand : IFileUploadCommand
    {
        private readonly IFileCreator _fileCreator;

        public FileUploadCommand(IFileCreator fileCreator)
        {
            _fileCreator = fileCreator;
        }

        public string Execute(Stream file, string filename)
        {
            var id = Guid.NewGuid().ToString();

            var generatedFileName = FileNameHelper.CreateSourceFileName(filename);
            var foldersPath = FileNameHelper.GetFoldersPathByFileId(id);
            UploadFile(file, generatedFileName, foldersPath);

            return id;
        }

        private void UploadFile(Stream file, string filename, IEnumerable<string> folders)
        {
            var createFile = CreateModelFile(file, filename, folders);
            _fileCreator.Create(createFile);
        }

        private static CreateFileModel CreateModelFile(Stream stream, string filename, IEnumerable<string> folders)
        {
            stream.Position = 0;
            var createFile = new CreateFileModel()
            {
                FileName = filename,
                FileStream = stream,
                FolderNames = folders
            };
            return createFile;
        }
    }
}
