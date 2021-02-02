using System.IO;
using System.Threading.Tasks;

namespace MRPlatform.VideoServer.Grpc.Tests.Helpers
{
    public static class FileHelper
    {
        public static Stream GetFileStream(string fileName)
        {
            var ms = new MemoryStream();
            using var file = File.OpenRead(fileName);
            file.CopyTo(ms);
            file.Close();

            return ms;
        }
    }
}
