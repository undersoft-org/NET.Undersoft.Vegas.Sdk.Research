
namespace System.Extract
{
    public static class ByteArrayExtractExtenstion
    {
        public static unsafe void CopyBlock(this byte[] src, byte[] dest, uint count)
        {
            Extractor.CopyBlock(src, dest, 0, count);
        }
        public static unsafe void CopyBlock(this byte[] src, byte[] dest, uint offset, uint count)
        {
            Extractor.CopyBlock(src, dest, offset, count);
        }

        public static unsafe void CopyBlock(this byte[] src, byte[] dest, ulong count)
        {
            Extractor.CopyBlock(src, dest, 0, count);
        }
        public static unsafe void CopyBlock(this byte[] src, byte[] dest, ulong offset, ulong count)
        {
            Extractor.CopyBlock(src, dest, offset, count);
        }

        public static object NewStructure(this byte[] binary, Type structure, long offset = 0)
        {
            return Extractor.BytesToStructure(binary, structure, offset);
        }

        public unsafe static object ToStructure(this byte[] binary, object structure, long offset = 0)
        {            
          return  Extractor.BytesToStructure(binary, structure, offset);
        }
        public unsafe static ValueType ToStructure(this byte[] binary, ValueType structure, long offset = 0)
        {
           return Extractor.BytesToStructure(binary, structure, offset);
        }

        public unsafe static ulong ToUInt64(this Byte[] bytes)
        {
            ulong v = 0;
            uint l = (uint)bytes.Length;
            fixed (byte* pbyte = bytes)
            {
                if (l < 8)
                {
                    byte* a = stackalloc byte[8];
                    Extractor.CopyBlock(a, pbyte, l);
                    v = *((ulong*)a);

                }
                v = *((ulong*)pbyte);
            }
            return v;
        }
        public unsafe static long ToInt64(this Byte[] bytes)
        {
            long v = 0;
            uint l = (uint)bytes.Length;
            fixed (byte* pbyte = bytes)
            {                
                if (l < 8)
                {
                    byte* a = stackalloc byte[8];
                    Extractor.CopyBlock(a, pbyte, l);
                    v = *((long*)a);

                }
                v = *((long*)pbyte);
            }
            return v;
        }

        public unsafe static uint ToUInt32(this Byte[] bytes)
        {
            uint v = 0;
            uint l = (uint)bytes.Length;
            fixed (byte* pbyte = bytes)
            {
                if (l < 4)
                {
                    byte* a = stackalloc byte[4];
                    Extractor.CopyBlock(a, pbyte, l);
                    v = *((uint*)a);

                }
                v = *((uint*)pbyte);
            }
            return v;
        }
        public unsafe static int ToInt32(this Byte[] bytes)
        {
            int v = 0;
            uint l = (uint)bytes.Length;
            fixed (byte* pbyte = bytes)
            {
                if (l < 4)
                {
                    byte* a = stackalloc byte[4];
                    Extractor.CopyBlock(a, pbyte, l);
                    v = *((int*)a);

                }
                v = *((int*)pbyte);
            }
            return v;
        }

        public unsafe static bool BlockEqual(this byte[] source, byte[] dest)
        {
            long sl = source.LongLength;
            if (sl > dest.LongLength)
                return false;
            long sl64 = sl / 8;
            long sl8 = sl % 8;
            fixed (byte* psrc = source, pdst = dest)
            {
                long* lsrc = (long*)psrc, ldst = (long*)pdst;
                for (int i = 0; i < sl64; i++)
                    if (*(&lsrc[i]) != (*(&ldst[i])))
                        return false;
                byte* psrc8 = psrc + (sl64 * 8), pdst8 = pdst + (sl64 * 8);
                for (int i = 0; i < sl8; i++)
                    if (*(&psrc8[i]) != (*(&pdst8[i])))
                        return false;
                return true;
            }            
        }
        public unsafe static bool BlockEqual(this IntPtr source, long srcOffset, IntPtr dest, long destOffset, long count)
        {
            long sl = count;
            long sl64 = sl / 8;
            long sl8 = sl % 8;
            long* lsrc = (long*)((byte*)source + srcOffset), ldst = (long*)((byte*)dest + destOffset);
            for (int i = 0; i < sl64; i++)
                if (*(&lsrc[i]) != (*(&ldst[i])))
                    return false;
            byte* psrc8 = (byte*)source + (sl64 * 8), pdst8 = (byte*)dest + (sl64 * 8);
            for (int i = 0; i < sl8; i++)
                if (*(&psrc8[i]) != (*(&pdst8[i])))
                    return false;
            return true;
        }
    }

}
