using System.IO;
using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;

namespace MRPlatform.VideoServer.Grpc.CQRS
{
    public interface IUploadFileStreamCommand
    {
        Task<UploadFileResponse> ExecuteAsync(IAsyncStreamReader<ChunkFile> requestStream);
    }

    public class UploadFileStreamCommand : IUploadFileStreamCommand
    {
        private readonly IFileUploadCommand _fileUploadCommand;

        public UploadFileStreamCommand(IFileUploadCommand fileUploadCommand)
        {
            _fileUploadCommand = fileUploadCommand;
        }

        public async Task<UploadFileResponse> ExecuteAsync(IAsyncStreamReader<ChunkFile> requestStream)
        {
            await using var ms = new MemoryStream();
            var chunkFile = await UploadToStreamAsync(requestStream, ms);

            var id = _fileUploadCommand.Execute(ms, chunkFile.FileName);

            return new UploadFileResponse()
            {
                FileId = id
            };
        }

        private async Task<ChunkFile> UploadToStreamAsync(IAsyncStreamReader<ChunkFile> requestStream, Stream destinationStream)
        {
            ChunkFile chunkFile = null;
            await foreach (var message in requestStream.ReadAllAsync())
            {
                message.Data.WriteTo(destinationStream);
                chunkFile = message;
            }

            return chunkFile;
        }
    }
}
