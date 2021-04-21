
namespace System
{
    public static class UInt64Extension
    {
        public static uint CountTrailingZeros(this ulong i)
        {
            return Bitscan.LengthBefore64(i);
        }

        public static uint CountLeadingZeros(this ulong i)
        {
            return Bitscan.LengthAfter64(i);
        }

        public static uint HighestBitId(this ulong i)
        {
            return Bitscan.ReverseIndex64(i);
        }

        public static uint LowestBitId(this ulong i)
        {
            return Bitscan.ForwardIndex64(i);
        }

        public static bool IsEven(this ulong i)
        {
            return !((i & 1UL) != 0);
        }

        public static bool IsOdd(this ulong i)
        {
            return ((i & 1UL) != 0);
        }
    }
}