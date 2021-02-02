using System.IO;

namespace MRPlatform.VideoServer.Common.FileProvider
{
    public class MediaFile
    {
        public string FileName { get; set; }

        public Stream Stream { get; set; }
    }
}
