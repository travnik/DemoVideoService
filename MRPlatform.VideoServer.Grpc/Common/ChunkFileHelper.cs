using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace MRPlatform.VideoServer.Grpc.Common
{
    public class ChunkFileHelper
    {
        protected static async Task WriteAsync(string fileName,
            IEnumerable<Stream> chunkStreams,
            IServerStreamWriter<ChunkFile> responseStream,
            CancellationToken cancellationToken)
        {
            foreach (var chunkStream in chunkStreams)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                chunkStream.Position = 0;
                var downloadModelResponse = new ChunkFile()
                {
                    FileName = fileName,
                    Data = await ByteString.FromStreamAsync(chunkStream, cancellationToken)
                };
                await responseStream.WriteAsync(downloadModelResponse);
            }
        }
    }
}
