using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace MRPlatform.VideoServer.Grpc.Common
{
    public interface IStreamChunksFlowSpliter
    {
        Task<IEnumerable<Stream>> SplitAsync(Stream stream, CancellationToken cancellationToken);
    }

    public class StreamChunksFlowSpliter : IStreamChunksFlowSpliter
    {
        private readonly ChunkSpliterOptions _options;

        public StreamChunksFlowSpliter(IOptions<ChunkSpliterOptions> options)
        {
            _options = options.Value;
        }

        public async Task<IEnumerable<Stream>> SplitAsync(Stream stream, CancellationToken cancellationToken)
        {
            stream.Position = 0;
            var chunkStreams = new List<Stream>();
            while (!cancellationToken.IsCancellationRequested && stream.Position < stream.Length)
            {
                var restOfLength = stream.Length - stream.Position;
                var size = restOfLength > _options.ChunkSize ? _options.ChunkSize : (int)restOfLength;

                var buffer = new byte[size];
                await stream.ReadAsync(buffer, 0, size, cancellationToken);
                var ms = new MemoryStream(buffer);
                chunkStreams.Add(ms);
            }

            return chunkStreams;
        }
    }
}
