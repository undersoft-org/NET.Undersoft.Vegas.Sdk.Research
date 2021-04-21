
namespace System
{
    public static class UInt32Extension
    {
        public static uint CountTrailingZeros(this uint i)
        {
            return Bitscan.LengthBefore32(i);
        }

        public static uint CountLeadingZeros(this uint i)
        {
            return Bitscan.LengthAfter32(i);
        }

        public static uint HighestBitId(this uint i)
        {
            return Bitscan.ReverseIndex32(i); 
        }

        public static uint LowestBitId(this uint i)
        {
            return Bitscan.ForwardIndex32(i);
        }

        public static bool IsEven(this uint i)
        {
            return !((i & 1) != 0);
        }

        public static bool IsOdd(this uint i)
        {
            return ((i & 1) != 0);
        }
    }
}