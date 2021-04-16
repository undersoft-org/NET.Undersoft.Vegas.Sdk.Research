using System.Runtime.InteropServices;

namespace System
{
    public static class Int32Extension
    {
        public static int NumberOfTrailingZeros(this int i)
        {
            int y;
            if (i == 0) return 32;
            int n = 31;
            y = i << 16; if (y != 0) { n -= 16; i = y; }
            y = i << 8; if (y != 0) { n = n - 8; i = y; }
            y = i << 4; if (y != 0) { n = n - 4; i = y; }
            y = i << 2; if (y != 0) { n = n - 2; i = y; }
            return n - ((i << 1) >> 31);
        }

        public static uint HighestOneBit(this uint i)
        {
            i |= (i >> 1);
            i |= (i >> 2);
            i |= (i >> 4);
            i |= (i >> 8);
            i |= (i >> 16);
            return i - (i >> 1);
        }
    }
}