using System.Runtime.InteropServices;

namespace System
{

    public static class IntPtrConversionExtension
    {
        #region Methods: Conversion

        public static T ElementAt<T>(this IntPtr ptr, int index)
        {
            var offset = Marshal.SizeOf(typeof(T)) * index;
            var offsetPtr = ptr.Increment(offset);
            return (T)Marshal.PtrToStructure(offsetPtr, typeof(T));
        }

        public static IntPtr ToIntPtr(this int value)
        {
            return new IntPtr(value);
        }

        public static IntPtr ToIntPtr(this uint value)
        {
            unchecked
            {
                return new IntPtr((int)value);
            }
        }

        public static IntPtr ToIntPtr(this long value)
        {
            unchecked
            {
                if (value > 0 && value <= 0xffffffff)
                    return new IntPtr((int)value);
            }

            return new IntPtr(value);
        }

        public static IntPtr ToIntPtr(this ulong value)
        {
            unchecked
            {
                return new IntPtr((long)value);
            }
        }

        /// <summary>
        /// Provides the current address of the given object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.PreserveSig)]
        public unsafe static System.IntPtr AddressOf(object obj)
        {
            if (obj == null) return System.IntPtr.Zero;

            System.TypedReference reference = __makeref(obj);

            System.TypedReference* pRef = &reference;

            return (IntPtr)pRef; //(&pRef)
        }

        /// <summary>
        /// Provides the current address of the given element
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.PreserveSig)]
        public unsafe static System.IntPtr AddressOf<T>(T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            System.TypedReference reference = __makeref(t);

            return *(IntPtr*)(&reference);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.PreserveSig)]
        public static unsafe System.IntPtr AddressOfRef<T>(ref T t)
        //refember ReferenceTypes are references to the CLRHeader
        //where TOriginal : struct
        {
            TypedReference reference = __makeref(t);

            TypedReference* pRef = &reference;

            return (IntPtr)pRef; //(&pRef)
        }

        public unsafe static UInt32 ToUInt32(this IntPtr pointer)
        {
            return (UInt32)((void*)pointer);
        }

        public unsafe static UInt64 ToUInt64(this IntPtr pointer)
        {
            return (UInt64)((void*)pointer);
        }
        #endregion                                 

    }
}
