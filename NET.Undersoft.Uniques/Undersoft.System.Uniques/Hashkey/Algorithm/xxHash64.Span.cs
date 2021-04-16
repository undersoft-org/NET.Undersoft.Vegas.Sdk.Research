namespace System.Uniques
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public static partial class xxHash64
    {
        public static unsafe ulong ComputeHash(Span<byte> data, int length, ulong seed = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);

            fixed (byte* pData = &MemoryMarshal.GetReference(data))
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }
        
        public static unsafe ulong ComputeHash(ReadOnlySpan<byte> data, int length, ulong seed = 0)
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
