using System.Collections.Generic;
using System.IO;

namespace MRPlatform.VideoServer.Common
{
    public static class FileNameHelper
    {
        public const string SourceFileName = "source_video_file";

        public static string CreateSourceFileName(string filename)
        {
            var fileExtension = Path.GetExtension(filename);

            return $"{SourceFileName}{fileExtension}";
        }

        public static string GetFullFilePathByFileId(string fileId, string extension)
        {
            var filename = GetFileName(fileId, extension);
            var filePath = Path.Combine(fileId.ToString(), filename);
            return filePath;
        }

        public static string GetFileName(string fileId, string extension)
        {
            return $"{fileId}{extension}";
        }

        public static IEnumerable<string> GetFoldersPathByFileId(string fileId)
        {
            return new List<string>() { GetFolderPathByFileId(fileId) };
        }

        public static string GetFolderPathByFileId(string fileId)
        {
            return fileId;
        }
    }
}
