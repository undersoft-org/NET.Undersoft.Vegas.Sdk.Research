using System.Extract.Stock;

namespace System.Extract.Stock
{
#if !NET40Plus
    /// <summary>
    /// Used when creating a memory mapped file
    /// </summary>
    public enum MemoryMappedFileAccess: uint
    {
        /// <summary>
        /// Read
        /// </summary>
        Read = 2,
        /// <summary>
        /// Read/Write
        /// </summary>
        ReadWrite = 4,
        /// <summary>
        /// CopyOnWrite
        /// </summary>
        CopyOnWrite = 8,
        /// <summary>
        /// Read Execute
        /// </summary>
        ReadExecute = 32,
        /// <summary>
        /// Read/Write Execute
        /// </summary>
        ReadWriteExecute = 64
    }

    internal static class MemoryMappedFileAccessExtensions
    {
        internal static UnsafeNative.FileMapAccess ToMapViewFileAccess(this MemoryMappedFileAccess access)
        {
            switch (access)
            {
                case MemoryMappedFileAccess.Read:
                    return UnsafeNative.FileMapAccess.FileMapRead;
                case MemoryMappedFileAccess.ReadWrite:
                    return UnsafeNative.FileMapAccess.FileMapRead | UnsafeNative.FileMapAccess.FileMapWrite;
                case MemoryMappedFileAccess.ReadExecute:
                    return UnsafeNative.FileMapAccess.FileMapRead | UnsafeNative.FileMapAccess.FileMapExecute;
                case MemoryMappedFileAccess.ReadWriteExecute:
                    return UnsafeNative.FileMapAccess.FileMapRead | UnsafeNative.FileMapAccess.FileMapWrite | UnsafeNative.FileMapAccess.FileMapExecute;
                default:
                    return UnsafeNative.FileMapAccess.FileMapAllAccess;
            }
        }
    }
#endif
}
