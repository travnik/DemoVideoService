using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileProvider.Api;
using Google.Protobuf;
using MRPlatform.VideoServer.Common;
using MRPlatform.VideoServer.Grpc.Tests.Helpers.Base;
using Xunit;

namespace MRPlatform.VideoServer.Grpc.Tests.Tests.Upload
{
    public class VideoServerService_UploadTests : GrpcTestsBase
    {
        private const int ChunkSize = 1024 * 1024 * 2;

        private readonly VideoServer.VideoServerClient _client;

        public VideoServerService_UploadTests()
        {
            _client = new VideoServer.VideoServerClient(Channel);
        }

        [Fact]
        public async Task UploadFileStream()
        {
            var fileRepo = GetRequiredService<IFileRepository>();
            var filename = $"{Guid.NewGuid()}.test";
            var stream = new MemoryStream();
            var data = new byte[10000000];
            stream.Write(data);

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var response = await UploadAsync(filename, stream, cts.Token);

            Assert.NotNull(response);
            Assert.True(fileRepo.IsExists($"{response.FileId}/{FileNameHelper.SourceFileName}.test"));
        }

        private async Task<UploadFileResponse> UploadAsync(string fileName)
        {
            using (var call = _client.UploadFileStream())
            {
                for (byte i = 0; i < 3; i++)
                {
                    var chunk = new byte[] {i, i, i};
                    var chunkModel = new ChunkFile()
                    {
                        FileName = fileName,
                    };
                    chunkModel.Data = ByteString.CopyFrom(chunk);
                    await call.RequestStream.WriteAsync(chunkModel);
                }

                await call.RequestStream.CompleteAsync();

                var response = await call;
                return response;
            }
        }

        private async Task<IEnumerable<Stream>> ToChunkStreamsAsync(Stream stream, CancellationToken cancellationToken)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var chunkStreams = new List<Stream>();
            while (!cancellationToken.IsCancellationRequested && stream.Position < stream.Length)
            {
                var restOfLength = stream.Length - stream.Position;
                var size = restOfLength > ChunkSize ? ChunkSize : (int) restOfLength;

                var buffer = new byte[size];
                await stream.ReadAsync(buffer, 0, size, cancellationToken);
                var ms = new MemoryStream(buffer);
                chunkStreams.Add(ms);
            }

            return chunkStreams;
        }

        private async Task<UploadFileResponse> UploadAsync(string fileName, Stream stream, CancellationToken cancellationToken)
        {
            var chunkStreams = await ToChunkStreamsAsync(stream, cancellationToken);
            using (var call = _client.UploadFileStream())
            {
                foreach (var chunkStream in chunkStreams)
                {
                    chunkStream.Seek(0, SeekOrigin.Begin);
                    var chunkModel = new ChunkFile()
                    {
                        FileName = fileName,
                        Data = await ByteString.FromStreamAsync(chunkStream, cancellationToken)
                    };
                    await call.RequestStream.WriteAsync(chunkModel);
                }

                await call.RequestStream.CompleteAsync();

                var response = await call;
                return response;
            }
        }
    }
}
