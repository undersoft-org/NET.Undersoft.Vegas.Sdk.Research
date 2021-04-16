using System.Extract.Stock;
using System;
using System.Security.Permissions;

namespace Microsoft.Win32.SafeHandles
{
#if !NET40Plus
    /// <summary>
    /// Provides a safe handle that represents a view of a block of unmanaged memory for random access.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public sealed class SafeMemoryMappedViewHandle: SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeMemoryMappedViewHandle()
            : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeMemoryMappedViewHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        /// <summary>
        /// Unmap's the view of the file
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            try
            {
                return UnsafeNative.UnmapViewOfFile(this.handle);
            }
            finally
            {
                this.handle = IntPtr.Zero;
            }
            
        }

        /// <summary>
        /// Acquires a reference to the pointer, incrementing the internal ref count. Should be followed by corresponding call to <see cref="ReleaseIntPtr"/>
        /// </summary>
        /// <param name="pointer"></param>
        public unsafe void AcquireIntPtr(ref byte* pointer)
        {
            bool flag = false;
            base.DangerousAddRef(ref flag);
            pointer = (byte*)this.handle.ToPointer();
        }

        /// <summary>
        /// Release the pointer
        /// </summary>
        public void ReleaseIntPtr()
        {
            base.DangerousRelease();
        }
    }
#endif
}
