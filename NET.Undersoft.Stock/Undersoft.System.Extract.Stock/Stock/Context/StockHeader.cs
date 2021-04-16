using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Extract.Stock
{
    /// <summary>
    /// A structure that is always located at the start of the shared memory in a <see cref="Stock"/> instance. 
    /// This allows the shared memory to be opened by other instances without knowing its size before hand.
    /// </summary>
    /// <remarks>This structure is the same size on 32-bit and 64-bit architectures.</remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct StockHeader
    {
        public long SharedMemorySize;
        public long UsedMemorySize;
        public long FreeMemorySize;

        public long ItemCapacity;
        public int  ItemSize;
        public int  ItemCount;

        public ushort StockId;
        public ushort SectorId;

        public volatile int Shutdown;
    }
}
