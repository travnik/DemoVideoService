using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace MRPlatform.VideoServer.Tests.Tests
{
    public class CreateThumbnailByVideoCommandTests
    {
        [Fact]
        public async Task CreateThumbnail()
        {
            var createThumbnailByVideoCommand = TestFactory.GetCreateThumbnailByVideoCommand();
            await createThumbnailByVideoCommand.ExecuteAsync(TestFactory.InputFilePath, TestFactory.OutputJpegFilePath);
            Assert.True(File.Exists(TestFactory.OutputJpegFilePath));
        }
    }
}
