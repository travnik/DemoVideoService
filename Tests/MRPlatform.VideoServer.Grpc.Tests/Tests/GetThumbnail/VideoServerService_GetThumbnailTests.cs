using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;
using MRPlatform.VideoServer.Grpc.Tests.Helpers;
using MRPlatform.VideoServer.Grpc.Tests.Helpers.Base;
using Xunit;

namespace MRPlatform.VideoServer.Grpc.Tests.Tests.GetThumbnail
{
    public class VideoServerService_GetThumbnailTests : GrpcTestsBase
    {
        private readonly VideoServer.VideoServerClient _client;
        private readonly IFileUploadCommand _fileUploadCommand;

        public VideoServerService_GetThumbnailTests()
        {
            _client = new VideoServer.VideoServerClient(Channel);
            _fileUploadCommand = GetRequiredService<IFileUploadCommand>();
        }

        [Fact]
        public async Task GetThumbnail()
        {
            using var file = FileHelper.GetFileStream(TestConst.FileNameVideoM4v);
            var fileId = _fileUploadCommand.Execute(file, TestConst.FileNameVideoM4v);

            var getRequest = new GetThumbnailRequest()
            {
                FileId = fileId.ToString()
            };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            using var streamingCall = _client.GetThumbnail(getRequest, cancellationToken: cts.Token);
            var chunks = new List<ChunkFile>();
            await foreach (var factModel in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
            {
                chunks.Add(factModel);
            }

            Assert.True(chunks.Sum(o => o.Data.Length) > 10);
        }
    }
}
