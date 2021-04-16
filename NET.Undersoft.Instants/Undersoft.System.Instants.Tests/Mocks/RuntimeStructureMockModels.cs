using System.Uniques;
using System.Linq;
using System.Runtime.InteropServices;
using System.Extract;
using Xunit;
using System.Globalization;

namespace System.Instants
{

    [StructLayout(LayoutKind.Sequential)]
    public class FieldsOnlyModel
    {
        public int Id = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "ProperSize";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name = "SizeIsTwoTimesLonger";

        public long Key = long.MaxValue;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray = new byte[10];

        public Ussn SerialCode = Ussn.Empty;

        public bool Status;

        public DateTime Time = DateTime.Now;

        public Guid GlobalId = new Guid();

        public double Factor = 2 * (long)int.MaxValue;

    }

    [StructLayout(LayoutKind.Sequential)]
    public class PropertiesOnlyModel
    {
        public int Id { get; set; } = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "ProperSize";

        [Structuring(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name { get; set; } = "SizeIsTwoTimesLonger";

        private long Key = long.MaxValue;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U1)]
        public byte[] ByteArray { get; set; }

        public Ussn SerialCode { get; set; } = Ussn.Empty;

        public bool Status { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public Guid GlobalId { get; set;} = new Guid();

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
    public class FieldsAndPropertiesModel
    {
        public int Id { get; set; } = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "ProperSize";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Name = "SizeIsTwoTimesLonger";

        private long Key = long.MaxValue;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray { get; set; } = new byte[10];

        public Ussn SerialCode { get; set; } = Ussn.Empty;

        public bool Status { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public Guid GlobalId { get; set; } = new Guid();

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

    }

  
}
