using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;
using MRPlatform.VideoServer.Grpc.Common;

namespace MRPlatform.VideoServer.Grpc.CQRS
{
    public interface IGetVideoStreamCommand
    {
        Task ExecuteAsync(GetVideoRequest request,
            IServerStreamWriter<ChunkFile> responseStream,
            CancellationToken cancellationToken);
    }

    public class GetVideoStreamCommand : ChunkFileHelper, IGetVideoStreamCommand
    {
        private readonly IStreamChunksFlowSpliter _streamChunksFlowSpliter;
        private readonly IGetVideoMpeg4Command _getVideoMpeg4Command;

        public GetVideoStreamCommand(IStreamChunksFlowSpliter streamChunksFlowSpliter, 
            IGetVideoMpeg4Command videoMpeg4Command)
        {
            _streamChunksFlowSpliter = streamChunksFlowSpliter;
            _getVideoMpeg4Command = videoMpeg4Command;
        }

        public async Task ExecuteAsync(GetVideoRequest request,
            IServerStreamWriter<ChunkFile> responseStream,
            CancellationToken cancellationToken)
        {
            var videoFile = await _getVideoMpeg4Command.ExecuteAsync(request.FileId);
            var chunkStreams = await _streamChunksFlowSpliter.SplitAsync(videoFile.Stream, cancellationToken);

            await WriteAsync(videoFile.FileName, chunkStreams, responseStream, cancellationToken);
        }
    }
}
