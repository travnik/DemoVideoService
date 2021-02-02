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

namespace MRPlatform.VideoServer.Grpc.Tests.Tests.GetVideo
{
    public class VideoServerService_GetVideoTests : GrpcTestsBase
    {
        private readonly VideoServer.VideoServerClient _client;
        private readonly IFileUploadCommand _fileUploadCommand;

        public VideoServerService_GetVideoTests()
        {
            _client = new VideoServer.VideoServerClient(Channel);
            _fileUploadCommand = GetRequiredService<IFileUploadCommand>();
        }

        [Fact]
        public async Task GetVideo()
        {
            using var file = FileHelper.GetFileStream(TestConst.FileNameVideoM4v);
            var fileId = _fileUploadCommand.Execute(file, TestConst.FileNameVideoM4v);

            var getVideoRequest = new GetVideoRequest()
            {
                FileId = fileId.ToString()
            };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            using var streamingCall = _client.GetVideo(getVideoRequest, cancellationToken: cts.Token);
            var chunks = new List<ChunkFile>();
            await foreach (var factModel in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
            {
                chunks.Add(factModel);
            }

            Assert.True(chunks.Sum(o => o.Data.Length) > 1000);
        }

        [Fact]
        public async Task GetVideo_Mp4()
        {
            using var file = FileHelper.GetFileStream(TestConst.FileNameVideoMp4);
            var fileId = _fileUploadCommand.Execute(file, TestConst.FileNameVideoMp4);
            var getVideoRequest = new GetVideoRequest()
            {
                FileId = fileId.ToString()
            };
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(300));

            using var streamingCall = _client.GetVideo(getVideoRequest, cancellationToken: cts.Token);
            var chunks = new List<ChunkFile>();
            await foreach (var factModel in streamingCall.ResponseStream.ReadAllAsync(cancellationToken: cts.Token))
            {
                chunks.Add(factModel);
            }

            Assert.True(chunks.Sum(o => o.Data.Length) > 1000);
        }
    }
}
