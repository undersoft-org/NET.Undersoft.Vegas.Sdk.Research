using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Extract.Stock
{
#if !NET40Plus
    /// <summary>
    /// Used for opening a memory-mapped file
    /// </summary>
    [Flags]
    public enum MemoryMappedFileRights: uint
    {
        /// <summary>The right to add data to a file or remove data from a file.</summary>
        Write = 0x02,
        /// <summary>The right to open and copy a file as read-only.</summary>
        Read = 0x04,
        /// <summary>The right to open and copy a file, and the right to add data to a file or remove data from a file.</summary>
        ReadWrite = MemoryMappedFileRights.Write | MemoryMappedFileRights.Read,
    }
#endif
}
