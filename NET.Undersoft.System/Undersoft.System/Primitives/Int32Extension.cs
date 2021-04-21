
namespace System
{
    public static class Int32Extension
    {
        public static uint CountTrailingZeros(this int i)
        {
            return Bitscan.LengthBefore32((uint)i);
        }

        public static uint CountLeadingZeros(this int i)
        {
            return Bitscan.LengthAfter32((uint)i);
        }

        public static uint HighestBitId(this int i)
        {
            return Bitscan.ReverseIndex32((uint)i); 
        }

        public static uint LowestBitId(this int i)
        {
            return Bitscan.ForwardIndex32((uint)i);
        }

        public static int RemoveSign(this int i)
        {
            return (int)(((uint)i << 1) >> 1);
        }

        public static bool IsEven(this int i)
        {
            return !((i & 1) != 0);
        }

        public static bool IsOdd(this int i)
        {
            return ((i & 1) != 0);
        }
    }
}