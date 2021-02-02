using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace MRPlatform.VideoServer.Tests.Tests
{
    public class VideoConverterTest
    {
        [Fact]
        public async Task Convert()
        {
            var converter = TestFactory.GetVideoConverter();

            await converter.ConvertAsync(TestFactory.InputFilePath, TestFactory.OutputFilePath);

            Assert.True(File.Exists(TestFactory.OutputFilePath));
        }

        [Fact]
        public async Task Convert_Mov()
        {
            var converter = TestFactory.GetVideoConverter();

            await converter.ConvertAsync(TestFactory.InputFilePathMov, TestFactory.OutputFilePath);

            Assert.True(File.Exists(TestFactory.OutputFilePath));
        }
    }
}
