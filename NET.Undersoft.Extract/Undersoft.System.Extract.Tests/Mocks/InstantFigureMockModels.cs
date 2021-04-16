using System.Uniques;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using Xunit;
using System.Globalization;
using System.Instants;

namespace System.Extract
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

        public Guid GlobalId { get; set; } = new Guid();

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

    }

    [StructLayout(LayoutKind.Sequential)]
    public class FieldsAndPropertiesModel
    {
        public int Id { get; set; } = 404;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Alias = "ProperSize";

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Name = "SizeIsTwoTimesLonger";

        private long Key = long.MaxValue;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10, ArraySubType = UnmanagedType.U1)]
        public byte[] ByteArray { get; set; }

        public Ussn SerialCode { get; set; } = Ussn.Empty;

        public bool Status { get; set; }

        public DateTime Time { get; set; } = DateTime.Now;

        public Guid GlobalId { get; set; } = new Guid();

        public double Factor { get; set; } = 2 * (long)int.MaxValue;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct StructModel
    {

        public StructModel(int id = 0)
        {
            Id = id;
            _alias = new char[10];
            name = new char[10];
            ByteArray = new byte[10];
            Key = 0;
            SerialCode = Ussn.Empty;
            Status = false;
            Time = DateTime.Now;
            GlobalId = Guid.Empty;
            Factor = 0;
        }

        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public char[] _alias;
        public unsafe string Alias
        {
            get
            {
                return new string(_alias);
            }
            set
            {
                if (_alias == null)
                    _alias = new char[10];
                int al = _alias.Length;
                int l = value.Length > _alias.Length ? _alias.Length : value.Length;
                int s = sizeof(char);
                fixed (char* v = value, a = _alias)
                    Extractor.Cpblk((byte*)a, (byte*)v, (uint)(l * s));
            }
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public char[] name;
        public unsafe string Name
        {
            get
            {
                return new string(name);
            }
            set
            {
                if (name == null)
                    name = new char[10];
                int al = name.Length;
                int l = value.Length > name.Length ? name.Length : value.Length;                
                int s = sizeof(char);
                fixed (char* v = value, a = name)
                    Extractor.Cpblk((byte*)a, (byte*)v, (uint)(l * s));
            }
        }

        public long Key;

        [Structuring(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] ByteArray;

        public Ussn SerialCode { get; set; }

        public bool Status { get; set; }

        public DateTime Time { get; set; }

        public Guid GlobalId { get; set; }

        public double Factor { get; set; }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct StructModels
    {
        public StructModels(StructModel[] structs)
        {
            Structs = structs;
        }

        [Structuring(UnmanagedType.LPArray, SizeConst = 3)]
        public StructModel[] Structs;
    }

}
