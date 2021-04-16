
namespace System
{

    public static class IntPtrComparisionExtension
    {
      
        public static Int32 CompareTo(this IntPtr left, Int32 right)
        {
            return left.CompareTo((UInt32)right);
        }

        public static Int32 CompareTo(this IntPtr left, IntPtr right)
        {
            if (left.ToUInt64() > right.ToUInt64())
                return 1;

            if (left.ToUInt64() < right.ToUInt64())
                return -1;

            return 0;
        }

        public static Int32 CompareTo(this IntPtr left, UInt32 right)
        {
            if (left.ToUInt64() > right)
                return 1;

            if (left.ToUInt64() < right)
                return -1;

            return 0;
        }
    }
}