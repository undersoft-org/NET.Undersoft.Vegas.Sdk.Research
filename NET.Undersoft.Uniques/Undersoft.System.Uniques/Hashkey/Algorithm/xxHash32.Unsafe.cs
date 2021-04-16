namespace System.Uniques
{
    using System.Runtime.CompilerServices;

    public static partial class xxHash32
    {
        private const uint p1 = 1024243321U;
        private const uint p2 = 2412134003U;
        private const uint p3 = 3259714649U;
        private const uint p4 = 949889989U;
        private const uint p5 = 573206377U;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe uint UnsafeComputeHash(byte* ptr, int length, uint seed)
        {
            byte* end = ptr + length;
            uint h32;

            if (length >= 16)
            {
                byte* limit = end - 16;

                uint v1 = seed + p1 + p2;
                uint v2 = seed + p2;
                uint v3 = seed + 0;
                uint v4 = seed - p1;

                do
                {
                    v1 += *((uint*)ptr) * p2;
                    v1 = (v1 << 13) | (v1 >> (32 - 13)); 
                    v1 *= p1;
                    ptr += 4;

                    v2 += *((uint*)ptr) * p2;
                    v2 = (v2 << 13) | (v2 >> (32 - 13)); 
                    v2 *= p1;
                    ptr += 4;

                    v3 += *((uint*)ptr) * p2;
                    v3 = (v3 << 13) | (v3 >> (32 - 13));
                    v3 *= p1;
                    ptr += 4;

                    v4 += *((uint*)ptr) * p2;
                    v4 = (v4 << 13) | (v4 >> (32 - 13)); 
                    v4 *= p1;
                    ptr += 4;

                } while (ptr <= limit);

                h32 = ((v1 << 1) | (v1 >> (32 - 1))) + 
                      ((v2 << 7) | (v2 >> (32 - 7))) +  
                      ((v3 << 12) | (v3 >> (32 - 12))) + 
                      ((v4 << 18) | (v4 >> (32 - 18))); 
            }
            else
            {
                h32 = seed + p5;
            }

            h32 += (uint)length;

            while (ptr <= end - 4)
            {
                h32 += *((uint*)ptr) * p3;
                h32 = ((h32 << 17) | (h32 >> (32 - 17))) * p4; 
                ptr += 4;
            }

            while (ptr < end)
            {
                h32 += *((byte*)ptr) * p5;
                h32 = ((h32 << 11) | (h32 >> (32 - 11))) * p1; 
                ptr += 1;
            }
       
            h32 ^= h32 >> 15;
            h32 *= p2;
            h32 ^= h32 >> 13;
            h32 *= p3;
            h32 ^= h32 >> 16;

            return h32;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnsafeAlign(byte[] data, int l, ref uint v1, ref uint v2, ref uint v3, ref uint v4)
        {
            fixed (byte* pData = &data[0])
            {
                byte* ptr = pData;
                byte* limit = ptr + l;

                do
                {
                    v1 += *((uint*)ptr) * p2;
                    v1 = (v1 << 13) | (v1 >> (32 - 13));
                    v1 *= p1;
                    ptr += 4;

                    v2 += *((uint*)ptr) * p2;
                    v2 = (v2 << 13) | (v2 >> (32 - 13)); 
                    v2 *= p1;
                    ptr += 4;

                    v3 += *((uint*)ptr) * p2;
                    v3 = (v3 << 13) | (v3 >> (32 - 13)); 
                    v3 *= p1;
                    ptr += 4;

                    v4 += *((uint*)ptr) * p2;
                    v4 = (v4 << 13) | (v4 >> (32 - 13)); 
                    v4 *= p1;
                    ptr += 4;

                } while (ptr < limit);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe uint UnsafeFinal(byte[] data, int l, ref uint v1, ref uint v2, ref uint v3, ref uint v4, long length, uint seed)
        {
            fixed (byte* pData = &data[0])
            {
                byte* ptr = pData;
                byte* end = pData + l;
                uint h32;

                if (length >= 16)
                {
                    h32 = ((v1 << 1) | (v1 >> (32 - 1))) +  
                          ((v2 << 7) | (v2 >> (32 - 7))) +   
                          ((v3 << 12) | (v3 >> (32 - 12))) + 
                          ((v4 << 18) | (v4 >> (32 - 18))); 
                }
                else
                {
                    h32 = seed + p5;
                }

                h32 += (uint)length;

 
                while (ptr <= end - 4)
                {
                    h32 += *((uint*)ptr) * p3;
                    h32 = ((h32 << 17) | (h32 >> (32 - 17))) * p4; 
                    ptr += 4;
                }

                while (ptr < end)
                {
                    h32 += *((byte*)ptr) * p5;
                    h32 = ((h32 << 11) | (h32 >> (32 - 11))) * p1; 
                    ptr += 1;
                }

                h32 ^= h32 >> 15;
                h32 *= p2;
                h32 ^= h32 >> 13;
                h32 *= p3;
                h32 ^= h32 >> 16;

                return h32;
            }
        }
    }
}
