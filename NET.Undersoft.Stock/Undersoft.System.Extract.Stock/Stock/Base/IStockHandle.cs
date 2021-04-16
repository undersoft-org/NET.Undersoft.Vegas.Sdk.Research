using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.Extract.Stock
{
    public interface IStockHandle
    {
        void WriteStock();
        void ReadStock();
        bool TryReadStock();
        void OpenStock();
        bool TryOpenStock();
        void CloseStock();
        IStock Stock { get; set; }
    }
}
