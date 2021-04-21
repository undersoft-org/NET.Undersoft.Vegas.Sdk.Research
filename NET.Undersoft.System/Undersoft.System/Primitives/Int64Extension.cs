
namespace System
{
    public static class Int64Extension
    {
        public static uint CountTrailingZeros(this long i)
        {
            return Bitscan.LengthBefore64((ulong)i);
        }

        public static uint CountLeadingZeros(this long i)
        {
            return Bitscan.LengthAfter64((ulong)i);
        }

        public static uint HighestBitId(this long i)
        {
            return Bitscan.ReverseIndex64((ulong)i); 
        }

        public static uint LowestBitId(this long i)
        {
            return Bitscan.ForwardIndex64((ulong)i);
        }

        public static long RemoveSign(this long i)
        {          
            return (long)(((ulong)i << 1) >> 1);
        }

        public static bool IsEven(this long i)
        {
            return !((i & 1L) != 0);
        }

        public static bool IsOdd(this long i)
        {
            return ((i & 1) != 0);
        }
    }
}