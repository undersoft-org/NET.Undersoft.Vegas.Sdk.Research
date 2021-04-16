using System.Uniques;
using System.Runtime.InteropServices;

namespace System.Instants.Mathline
{   

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class MathlineMockModel
    {
        public int Id { get; set; } = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "StockAlias";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name = "StockFullName";

        private long Key = long.MaxValue;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray { get; set; } = new byte[10];

        public Ussn SerialCode { get; set; } = Ussn.Empty;

        public bool Status { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public Guid GlobalId { get; set; } = new Guid();

        public double NetPrice { get; set; } = 1.00;

        public double NetCost { get; set; } = 1.00;

        public double SellNetPrice { get; set; } = 1.00;

        public double SellGrossPrice { get; set; } = 0;

        public double BuyNetCost { get; set; } = 0.85;

        public double SellNetTotal { get; set; } = 1.00;

        public double SellGrossTotal { get; set; } = 0;

        public double BuyNetTotal { get; set; } = 0.85;

        public int    Quantity = 86;

        public double SellFeeRate { get; set; } = 8;

        public double BuyFeeRate { get; set; } = 8;

        public double TargetMarkupRate { get; set; } = 0;

        public double CurrentMarkupRate { get; set; } = 0;

        public double TaxRate { get; set; } = 1.23;

        public double CurrencyRate { get; set; } = 1.23;

    }

  
}
