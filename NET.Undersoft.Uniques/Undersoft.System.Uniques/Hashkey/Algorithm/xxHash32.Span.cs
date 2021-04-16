using System.Diagnostics;

namespace System.Uniques
{
    using System.Runtime.InteropServices;

    public static partial class xxHash32
    {
        public static unsafe uint ComputeHash(Span<byte> data, int length, uint seed = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);

            fixed (byte* pData = &MemoryMarshal.GetReference(data))
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        
        public static unsafe uint ComputeHash(ReadOnlySpan<byte> data, int length, uint seed = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);

            fixed (byte* pData = &MemoryMarshal.GetReference(data))
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
    }
}
