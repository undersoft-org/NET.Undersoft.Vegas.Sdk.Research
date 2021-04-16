using System.Extract;

namespace System.Uniques
{
    using System.Buffers;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class xxHash32
    {
        public static async ValueTask<uint> ComputeHashAsync(Stream stream, int bufferSize = 4096, uint seed = 0)
        {
            return await ComputeHashAsync(stream, bufferSize, seed, CancellationToken.None);
        }
              
        public static async ValueTask<uint> ComputeHashAsync(Stream stream, int bufferSize, uint seed, CancellationToken cancellationToken)
        {
            Debug.Assert(stream != null);
            Debug.Assert(bufferSize > 16);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize + 16);

            int readBytes;
            int offset = 0;
            long length = 0;

            uint v1 = seed + p1 + p2;
            uint v2 = seed + p2;
            uint v3 = seed + 0;
            uint v4 = seed - p1;

            try
            {
                while ((readBytes = await stream.ReadAsync(buffer, offset, bufferSize, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return await Task.FromCanceled<uint>(cancellationToken);
                    }
                    
                    length = length + readBytes;
                    offset = offset + readBytes;

                    if (offset < 16) continue;

                    int r = offset % 16;
                    int l = offset - r;  

                    UnsafeAlign(buffer, l, ref v1, ref v2, ref v3, ref v4);

                    Extractor.CopyBlock(buffer, 0, buffer, l, r);

                    offset = r;
                }

                uint h32 = UnsafeFinal(buffer, offset, ref v1, ref v2, ref v3, ref v4, length, seed);

                return h32;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
