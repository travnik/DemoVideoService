using System.Threading.Tasks;
using Grpc.Core;
using MRPlatform.VideoServer.CQRS;
using MRPlatform.VideoServer.Grpc.CQRS;

namespace MRPlatform.VideoServer.Grpc.Services
{
    public class VideoServerService : VideoServer.VideoServerBase
    {
        private readonly IUploadFileStreamCommand _uploadFileStreamCommand;
        private readonly IGetVideoStreamCommand _getVideoStreamCommand;
        private readonly IGetThumbnailCommand _getThumbnailCommand;
        private readonly IRemoveFilesByFileIdCommand _removeFilesCommand;

        public VideoServerService(IUploadFileStreamCommand uploadFileStreamCommand, 
            IGetVideoStreamCommand getVideoStreamCommand, 
            IGetThumbnailCommand getThumbnailCommand, 
            IRemoveFilesByFileIdCommand removeFilesCommand)
        {
            _uploadFileStreamCommand = uploadFileStreamCommand;
            _getVideoStreamCommand = getVideoStreamCommand;
            _getThumbnailCommand = getThumbnailCommand;
            _removeFilesCommand = removeFilesCommand;
        }

        public override Task<UploadFileResponse> UploadFileStream(IAsyncStreamReader<ChunkFile> requestStream, ServerCallContext context)
        {
            return _uploadFileStreamCommand.ExecuteAsync(requestStream);
        }

        public override Task GetVideo(GetVideoRequest request, IServerStreamWriter<ChunkFile> responseStream, ServerCallContext context)
        {
            return _getVideoStreamCommand.ExecuteAsync(request, responseStream, context.CancellationToken);
        }

        public override Task GetThumbnail(GetThumbnailRequest request, IServerStreamWriter<ChunkFile> responseStream, ServerCallContext context)
        {
            return _getThumbnailCommand.ExecuteAsync(request, responseStream, context.CancellationToken);
        }

        public override Task<Empty> Remove(RemoveRequest request, ServerCallContext context)
        {
            _removeFilesCommand.Execute(request.FileId);

            return Task.FromResult(new Empty());
        }
    }
}
