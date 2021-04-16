using System.Runtime.InteropServices;

namespace System
{

    public static class IntPtrArithmeticExtension
    {
        #region Methods: Arithmetics
        public static IntPtr Decrement(this IntPtr pointer, Int32 value)
        {
            return Increment(pointer, -value);
        }

        public static IntPtr Decrement(this IntPtr pointer, Int64 value)
        {
            return Increment(pointer, -value);
        }

        public static IntPtr Decrement(this IntPtr pointer, IntPtr value)
        {
            switch (IntPtr.Size)
            {
                case sizeof(Int32):
                    return (new IntPtr(pointer.ToInt32() - value.ToInt32()));

                default:
                    return (new IntPtr(pointer.ToInt64() - value.ToInt64()));
            }
        }

        public static IntPtr Increment(this IntPtr pointer, Int32 value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(Int32):
                        return (new IntPtr(pointer.ToInt32() + value));

                    default:
                        return (new IntPtr(pointer.ToInt64() + value));
                }
            }
        }

        public static IntPtr Increment(this IntPtr pointer, Int64 value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(Int32):
                        return (new IntPtr((Int32)(pointer.ToInt32() + value)));

                    default:
                        return (new IntPtr(pointer.ToInt64() + value));
                }
            }
        }

        public static IntPtr Increment(this IntPtr pointer, IntPtr value)
        {
            unchecked
            {
                switch (IntPtr.Size)
                {
                    case sizeof(int):
                        return new IntPtr(pointer.ToInt32() + value.ToInt32());
                    default:
                        return new IntPtr(pointer.ToInt64() + value.ToInt64());
                }
            }
        }

        public static IntPtr Increment<T>(this IntPtr ptr)
        {
            return ptr.Increment(Marshal.SizeOf(typeof(T)));
        }

        #endregion

    }
}