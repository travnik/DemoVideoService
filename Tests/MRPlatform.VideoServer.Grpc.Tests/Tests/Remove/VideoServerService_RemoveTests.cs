using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;
using MRPlatform.VideoServer.Grpc.Tests.Helpers;
using MRPlatform.VideoServer.Grpc.Tests.Helpers.Base;
using Xunit;

namespace MRPlatform.VideoServer.Grpc.Tests.Tests.Remove
{
    public class VideoServerService_RemoveTests : GrpcTestsBase
        {
        private readonly VideoServer.VideoServerClient _client;
        private readonly IFileUploadCommand _fileUploadCommand;

        public VideoServerService_RemoveTests()
        {
            _client = new VideoServer.VideoServerClient(Channel);
            _fileUploadCommand = GetRequiredService<IFileUploadCommand>();
        }

        [Fact]
        public async Task Remove()
        {
            using var file = FileHelper.GetFileStream(TestConst.FileNameVideoM4v);
            var fileId = _fileUploadCommand.Execute(file, TestConst.FileNameVideoM4v);

            var request = new RemoveRequest()
            {
                FileId = fileId.ToString()
            };
            var response = await _client.RemoveAsync(request);
            Assert.NotNull(response);
        }
    }
}
