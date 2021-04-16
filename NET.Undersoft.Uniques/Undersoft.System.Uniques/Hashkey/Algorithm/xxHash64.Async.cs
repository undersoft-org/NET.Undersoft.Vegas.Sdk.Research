using System.Extract;

namespace System.Uniques
{
    using System.Buffers;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class xxHash64
    {
        public static async ValueTask<ulong> ComputeHashAsync(Stream stream, int bufferSize = 8192, ulong seed = 0)
        {
            return await ComputeHashAsync(stream, bufferSize, seed, CancellationToken.None);
        }
        
        public static async ValueTask<ulong> ComputeHashAsync(Stream stream, int bufferSize, ulong seed, CancellationToken cancellationToken)
        {
            Debug.Assert(stream != null);
            Debug.Assert(bufferSize > 32);

            byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize + 32);

            int readBytes;
            int offset = 0;
            long length = 0;

            ulong v1 = seed + p1 + p2;
            ulong v2 = seed + p2;
            ulong v3 = seed + 0;
            ulong v4 = seed - p1;

            try
            {
                while ((readBytes = await stream.ReadAsync(buffer, offset, bufferSize, cancellationToken).ConfigureAwait(false)) > 0)
                {   
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return await Task.FromCanceled<ulong>(cancellationToken);
                    }
                    
                    length = length + readBytes;
                    offset = offset + readBytes;

                    if (offset < 32) continue;

                    int r = offset % 32; 
                    int l = offset - r;  
 
                    UnsafeAlign(buffer, l, ref v1, ref v2, ref v3, ref v4);

                    Extractor.CopyBlock(buffer, 0, buffer, l, r);
                    offset = r;
                }

                ulong h64 = UnsafeFinal(buffer, offset, ref v1, ref v2, ref v3, ref v4, length, seed);

                return h64;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
}
