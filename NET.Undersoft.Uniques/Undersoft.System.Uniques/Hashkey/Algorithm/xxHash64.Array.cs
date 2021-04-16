namespace System.Uniques
{
    using System.Diagnostics;

    public static partial class xxHash64
    {
        public static unsafe ulong ComputeHash(byte[] data, int length, ulong seed = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);
            
            fixed (byte* pData = &data[0])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }   

        public static unsafe ulong ComputeHash(byte[] data, int offset, int length, ulong seed = 0)
        {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(offset < data.Length);
            Debug.Assert(length <= data.Length - offset);

            fixed (byte* pData = &data[0 + offset])
            {
                return UnsafeComputeHash(pData, length, seed);
            }
        }   

        public static unsafe ulong ComputeHash(System.ArraySegment<byte> data, ulong seed = 0)
        {
            Debug.Assert(data != null);

            return ComputeHash(data.Array, data.Offset, data.Count, seed);
        }   

    }
}