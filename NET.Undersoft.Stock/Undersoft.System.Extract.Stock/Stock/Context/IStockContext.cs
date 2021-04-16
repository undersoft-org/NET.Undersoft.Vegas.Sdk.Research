using System.Net.Sockets;
using System.IO;
using System;

namespace System.Extract.Stock
{
    public interface IStockContext : IDisposable
    {
        string Place { get; set; }
        string File { get; set; }

        ushort StockId { get; set; }
        ushort SectorId { get; set; }

        long BufferSize { get; set; }
        long UsedSize { get; set; }
        long FreeSize { get; set; }

        int ItemSize { get; set; }
        int ItemCount { get; set; }
        long ItemCapacity { get; set; }

        int NodeCount { get; set; }
        int ServerCount { get; set; }
        int ClientCount { get; set; }
        int Elements { get; set; }
    }
}
