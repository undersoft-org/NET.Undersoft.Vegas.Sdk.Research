using System.Extract.Stock;
using System;
using System.Security.Permissions;

namespace Microsoft.Win32.SafeHandles
{
#if !NET40Plus
    /// <summary>
    /// Provides a safe handle that represents a memory-mapped file for sequential access.
    /// </summary>
    public sealed class SafeMemoryMappedFileHandle: SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeMemoryMappedFileHandle()
            : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeMemoryMappedFileHandle(IntPtr handle, bool ownsHandle)
            : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        /// <summary>
        /// Closes the memory-mapped file handle
        /// </summary>
        /// <returns></returns>
        protected override bool ReleaseHandle()
        {
            try
            {
                return UnsafeNative.CloseHandle(this.handle);
            }
            finally
            {
                this.handle = IntPtr.Zero;
            }
        }
    }
#endif
}
