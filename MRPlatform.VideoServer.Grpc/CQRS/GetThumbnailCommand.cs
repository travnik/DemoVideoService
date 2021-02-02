using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;
using MRPlatform.VideoServer.Grpc.Common;

namespace MRPlatform.VideoServer.Grpc.CQRS
{
    public interface IGetThumbnailCommand
    {
        Task ExecuteAsync(GetThumbnailRequest request, 
            IServerStreamWriter<ChunkFile> responseStream,
            CancellationToken cancellationToken);
    }

    public class GetThumbnailCommand : ChunkFileHelper, IGetThumbnailCommand
    {
        private readonly IStreamChunksFlowSpliter _streamChunksFlowSpliter;
        private readonly IGetThumbnailJpegCommand _thumbnailJpegCommand;

        public GetThumbnailCommand(IStreamChunksFlowSpliter streamChunksFlowSpliter, 
            IGetThumbnailJpegCommand thumbnailJpegCommand)
        {
            _streamChunksFlowSpliter = streamChunksFlowSpliter;
            _thumbnailJpegCommand = thumbnailJpegCommand;
        }

        public async Task ExecuteAsync(GetThumbnailRequest request, 
            IServerStreamWriter<ChunkFile> responseStream,
            CancellationToken cancellationToken)
        {
            var file = await _thumbnailJpegCommand.ExecuteAsync(request.FileId);
            var chunkStreams = await  _streamChunksFlowSpliter.SplitAsync(file.Stream, cancellationToken);

            await WriteAsync(file.FileName, chunkStreams, responseStream, cancellationToken);
        }
    }
}
