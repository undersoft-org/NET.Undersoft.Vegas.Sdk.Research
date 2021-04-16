using System.Runtime.InteropServices;
using System.Extract;

namespace System.Uniques
{
    [Serializable]
    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential)]    
    public unsafe struct Urpn : IFormattable, IComparable 
        , IComparable<Urpn>, IEquatable<Urpn>, IUnique       
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]  
        private byte[] bytes;

        public long KeyBlock
        {
            get
            {
                if (IsNull) return 0;
                fixed (byte* pbyte = bytes)
                    return *((long*)pbyte);

            }
            set
            {
                fixed (byte* b = SureBytes)
                    *((long*)b) = value;
            }
        }       

        public ushort KeyBlockX
        {
            get
            {
                if (IsNull) return 0;
                fixed (byte* pbyte = &bytes[8])
                    return *((ushort*)pbyte);
            }
            set
            {
                fixed (byte* b = &SureBytes[8])
                    *((ushort*)b) = value;
            }
        }

        public ushort KeyBlockY
        {
            get
            {
                if (IsNull) return 0;
                fixed (byte* pbyte = &bytes[16])
                    return *((ushort*)pbyte);
            }
            set
            {
                fixed (byte* b = &SureBytes[16])
                    *((ushort*)b) = value;
            }
        }

        public Urpn(long l)
        {
            bytes = new byte[24];
            fixed (byte* b = bytes)
            {
                *((long*)b) = l;
            }
        }
        public Urpn(string s)
        {          
            bytes = new byte[24];
            this.FromHexTetraChars(s.ToCharArray());    //RR
        }
        public Urpn(byte[] b)
        {
            bytes = new byte[24];
            if (b != null)
            {
                int l = b.Length;
                if (l > 24)
                    l = 24;
                b.CopyBlock(bytes, (uint)l);
            }
          
        }
         
        public Urpn(long x, long y, long z)
        {
            bytes = new byte[24];

            fixed (byte* n = bytes)
            {
                *((long*)n) = x;
                *((long*)&n[8]) = y;
                *((long*)&n[16]) = z;
            }
        }
        public Urpn(byte[] x, byte[] y, byte[] z)
        {
            bytes = new byte[24];
            fixed (byte* n = bytes)
            {
                fixed (byte* s = x)                
                    *((long*)n) = *((long*)s);
                fixed (byte* s = y)
                    *((long*)(n + 8)) = *((long*)s);
                fixed (byte* s = z)
                    *((long*)(n + 16)) = *((long*)s);       
            }
        }
        public Urpn(object x, object[] y, object[] z)
        {
            bytes = new byte[24];

            fixed (byte* n = bytes)
            {
                fixed(byte* s = x.GetHashBytes64())
                    *((long*)n) = *((long*)s);
                fixed (byte* s = y.GetHashBytes64())
                    *((long*)(n + 12)) = *((long*)s);
                fixed (byte* s = z.GetHashBytes64())
                    *((long*)(n + 16)) = *((long*)s);             
            }
        }

        public byte[] this[int offset]
        {
            get
            {
                if (offset != 0)
                {
                    byte[] r = new byte[24 - offset];
                    fixed (byte* pbyte = &NotSureBytes[offset])
                    fixed (byte* rbyte = r)
                    {
                        Extractor.CopyBlock(rbyte, pbyte, (uint)(24 - offset));
                    }
                    return r;
                }
                return NotSureBytes;
            }
            set
            {
                int l = value.Length;
                if (offset != 0 || l < 24)
                {
                    int count = 24 - offset;
                    if (l < count)
                        count = l;
                    value.CopyBlock(SureBytes, (uint)offset, (uint)count);
                }
                else
                {
                    value.CopyBlock(SureBytes, 0, 24);
                }
            }
        }
        public byte[] this[int offset, int length]
        {
            get
            {
                if (offset < 24)
                {
                    if ((24 - offset) > length)
                        length = 24 - offset;

                    byte[] r = new byte[length];
                    fixed (byte* pbyte = &NotSureBytes[offset])
                    fixed (byte* rbyte = r)
                    {
                        Extractor.CopyBlock(rbyte, pbyte, (uint)length);
                    }
                    return r;
                }
                return null;

            }
            set
            {
                if (offset < 24)
                {
                    if ((24 - offset) > length)
                        length = 24 - offset;
                    if (value.Length < length)
                        length = value.Length;

                    byte[] r = new byte[length];
                    fixed (byte* pbyte = value)
                    fixed (byte* rbyte = &SureBytes[offset])
                    {
                        Extractor.CopyBlock(rbyte, pbyte, (uint)length);
                    }
                }
            }
        }

        public void SetBytes(byte[] value, int offset)
        {
            this[offset] = value;
        }

        public byte[] GetBytes(int offset, int length)
        {
            return this[offset, length];
        }

        public byte[] GetBytes()
        {
            return SureBytes;
        }

        public byte[] GetKeyBytes()
        {
            byte[] kbytes = new byte[8];
            fixed (byte* b = SureBytes)
            fixed (byte* k = kbytes)
                *((long*)k) = *((long*)b);
            return kbytes;
        }

        //public long    ValueFromXYZ(short blockZ, int vectorZ, short blockY, int vectorY, short blockX)
        //{
        //    return (blockZ * vectorZ * vectorY) + (blockY * vectorY) + blockX;
        //}
        //public ushort[] ValueToXYZ(long vectorZ, long vectorY, long value)
        //{
        //    if (value > 0)
        //    {
        //        long vectorYZ = (vectorY * vectorZ);
        //        long blockZdiv = (value / vectorYZ);
        //        long blockYsub = value - (blockZdiv * vectorYZ);
        //        long blockYdiv = blockYsub / vectorY;
        //        long blockZ = (blockZdiv > 0 && (value % vectorYZ) > 0) ? blockZdiv + 1 : blockZdiv;
        //        long blockY = (blockYdiv > 0 && (value % vectorY) > 0) ? blockYdiv + 1 : blockYdiv;
        //        long blockX = value % vectorY;
        //        return new ushort[] { (ushort)blockZ, (ushort)blockY, (ushort)blockX };
        //    }
        //    return null;
        //}

        public bool IsNull
        {
            get
            {
                if (bytes == null)
                    return true;
                return false;
            }
            set
            {
                if (value) bytes = null;
            }
        }

        public bool IsNotEmpty
        {
            get { return (!IsNull && KeyBlock != 0); }
        }

        public bool IsEmpty
        {
            get { return (IsNull || KeyBlock == 0); }
        }

        public byte[] SureBytes
        {
            get => (bytes == null) ? bytes = new byte[24] : bytes;
        }

        public byte[] NotSureBytes
        {
            get => (bytes == null) ? new byte[24] : bytes;
        }

        public override int GetHashCode()
        {
            fixed (byte* pbyte = &this[0,8].BitAggregate64to32()[0])
                return *((int*)pbyte);
        }

        public void SetHashKey(long value)
        {
            KeyBlock = value;
        }

        public long GetHashKey()
        {
            return KeyBlock;
        }

        public int CompareTo(object value)
        {
            if (value == null)
                return 1;
            if (!(value is Urpn))
                throw new Exception();

            return (int)(KeyBlock - ((Urpn)value).KeyBlock);
        }

        public int CompareTo(Urpn g)
        {
            return (int)(KeyBlock - g.KeyBlock);
        }

        public int CompareTo(IUnique g)
        {
            return (int)(KeyBlock - g.KeyBlock);
        }

        public bool Equals(long g)
        {
            return (KeyBlock == g);
        }

        public override bool Equals(object value)
        {
            if (value == null || bytes == null)
                return false;
            if ((value is string))
                return new Urpn(value.ToString()).KeyBlock == KeyBlock;

            return (KeyBlock == ((Urpn)value).KeyBlock);
        }

        public bool Equals(Urpn g)
        {
            return (KeyBlock == g.KeyBlock);
        }
        public bool Equals(IUnique g)
        {
            return (GetHashKey() == g.GetHashKey());
        }

        public override String ToString()
        {
            if (bytes == null)
                bytes = new byte[24];
            return new string(this.ToHexTetraChars());  
        }

        public String ToString(String format)
        {
            return ToString(format, null);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (bytes == null)
                bytes = new byte[24];
            return new string(this.ToHexTetraChars());  //RR
        }

        public static bool operator ==(Urpn a, Urpn b)
        {
            return (a.KeyBlock == b.KeyBlock);
        }

        public static bool operator !=(Urpn a, Urpn b)
        {
            return (a.KeyBlock != b.KeyBlock);
        }

        public static explicit operator Urpn(String s)
        {
            return new Urpn(s);
        }
        public static implicit operator String(Urpn s)
        {
            return s.ToString();
        }
      

        public static explicit operator Urpn(byte[] l)
        {
            return new Urpn(l);
        }
        public static implicit operator byte[](Urpn s)
        {
            return s.GetBytes();
        }

        public static Urpn Empty
        {
            get { return new Urpn(new byte[24]); }
        }      

        IUnique IUnique.Empty
        {
            get
            {
                return new Urpn(new byte[24]);
            }
        }

        public char[] ToHexTetraChars()
        {
            char[] pchchar = new char[32];
            long pchblock;  
            int pchlength = 32;
            byte pchbyte;
            int idx = 0;

            for (int j = 0; j < 4; j++)
            {
                fixed (byte* pbyte = &bytes[j * 6])
                {
                    pchblock = *((long*)pbyte);
                }
                pchblock = pchblock & 0x0000ffffffffffffL;  //each block has 6 bytes
                for (int i = 0; i < 8; i++)
                {
                    pchbyte = (byte)(pchblock & 0x3fL);                    
                    pchchar[idx] = (pchbyte).ToHexTetraChar();
                    idx++;                    
                    pchblock = pchblock >> 6;
                    if (pchbyte != 0x00) pchlength = idx;
                }
            }
                        
            char[] pchchartrim = new char[pchlength];
            Array.Copy(pchchar, 0, pchchartrim, 0, pchlength);

            return pchchartrim;            
        }

        public void FromHexTetraChars(char[] pchchar)
        {
            int pchlength = pchchar.Length;
            int idx = 0;
            byte pchbyte;
            long pchblock = 0;
            int blocklength = 8;
            int pchblock_int;
            short pchblock_short;

            for (int j = 0; j < 4; j++)
            {
                pchblock = 0x00L;
                blocklength = Math.Min(8, Math.Max(0, pchlength - 8 * j));        //required if trimmed zeros, length < 32
                idx = Math.Min(pchlength, 8*(j+1)) - 1;                           //required if trimmed zeros, length <32

                for (int i = 0; i < blocklength; i++)     //8 chars per block, each 6 bits
                {
                    pchbyte = (pchchar[idx]).ToHexTetraByte();
                    pchblock = pchblock << 6;
                    pchblock = pchblock | (pchbyte & 0x3fL);
                    idx--;
                }
                fixed (byte* pbyte = bytes)
                {
                    if (j == 3) //ostatnie nalozenie - block3 przekracza o 2 bajty rozmiar bytes!!!! tych 2 bajty sa 0, ale uniknac ewentualne wejscia w pamiec poza bytes
                    {
                        pchblock_short = (short)(pchblock & 0x00ffffL);
                        pchblock_int = (int)(pchblock >> 16);
                        *((long*)&pbyte[18]) = pchblock_short;
                        *((long*)&pbyte[20]) = pchblock_int;
                        break;
                    }
                    *((long*)&pbyte[j * 6]) = pchblock;

                }
            }                                    
        }

        public bool EqualsContent(Urpn g)
        {
            if (g == null) return false;
            fixed (byte* gbyte = g.bytes)
            fixed (byte* pbyte = bytes)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (*((long*)&pbyte[i * 8]) != *((long*)&pbyte[i * 8]))
                        return false;
                }
            }
            return true;           
        }
      
    }
}
