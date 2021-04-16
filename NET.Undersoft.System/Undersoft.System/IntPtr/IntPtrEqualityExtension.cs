using System.Runtime.InteropServices;

namespace System
{

    public static class IntPtrEqualityExtension
    {
        #region Methods: Equality
        public static Boolean Equals(this IntPtr pointer, Int32 value)
        {
            return (pointer.ToInt32() == value);
        }

        public static Boolean Equals(this IntPtr pointer, Int64 value)
        {
            return (pointer.ToInt64() == value);
        }

        public static Boolean Equals(this IntPtr left, IntPtr ptr2)
        {
            return (left == ptr2);
        }

        public static Boolean Equals(this IntPtr pointer, UInt32 value)
        {
            return (pointer.ToUInt32() == value);
        }

        public static Boolean Equals(this IntPtr pointer, UInt64 value)
        {
            return (pointer.ToUInt64() == value);
        }

        public static Boolean Equals(this IntPtr[] left, IntPtr[] right)
        {
            int length = left.Length;
            for (int i = 0; i < length; i++)
                if (!left[i].Equals(right[i]))
                    return false;
            return true;
        }

        public static Boolean isGreaterThanOrEqualTo(this IntPtr left, IntPtr right)
        {
            return (left.CompareTo(right) >= 0);
        }

        public static Boolean IsLessThanOrEqualTo(this IntPtr left, IntPtr right)
        {
            return (left.CompareTo(right) <= 0);
        }
        #endregion

    }
}