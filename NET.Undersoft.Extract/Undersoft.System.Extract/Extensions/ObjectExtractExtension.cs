using System.Collections;
using System.Runtime.InteropServices;

namespace System.Extract
{
    public static class ObjectExtractExtenstion
    {
        public unsafe static object StructureFrom(this object structure, byte* binary)
        {
            return Extractor.PointerToStructure(binary, structure);
        }
        public unsafe static object StructureFrom(this object structure, IntPtr binary)
        {
            return Extractor.PointerToStructure(binary, structure);
        }
        public unsafe static object StructureFrom(this object structure, byte[] binary, long offset = 0)
        {
           return Extractor.BytesToStructure(binary, structure, offset);
        }

        public unsafe static void StructureTo(this object structure, byte* binary)
        {
            Extractor.StructureToPointer(structure, binary);
        }
        public unsafe static void StructureTo(this object structure, IntPtr binary)
        {
            Extractor.StructureToPointer(structure, binary);
        }
        public unsafe static void StructureTo(this object structure, ref byte[] binary, long offset = 0)
        {
            Extractor.StructureToBytes(structure, ref binary, offset);
        }

        public unsafe static byte[] GetStructureBytes(this object structure)
        {
            return Extractor.GetStructureBytes(structure);
        }
        public unsafe static byte[] GetSequentialBytes(this Object objvalue)
        {
            byte[] b = new byte[Marshal.SizeOf(objvalue)];
            fixed (byte* pb = b)
                Marshal.StructureToPtr(objvalue, new IntPtr(pb), false);
            return b;
        }

        public unsafe static byte* GetStructurePointer(this object structure)
        {
            return Extractor.GetStructurePointer(structure);
        }
        public unsafe static IntPtr GetStructureIntPtr(this object structure)
        {
            return Extractor.GetStructureIntPtr(structure);
        }

        public unsafe static long GetSize(this object structure)
        {
            return Extractor.GetSize(structure);
        }
        public unsafe static long[] GetSizes(this object structure)
        {
            return Extractor.GetSizes(structure);
        }

        public unsafe static Byte[] GetBytes(this Object objvalue)
        {            
            Type t = objvalue.GetType();
            if (objvalue is ValueType)
            {
                if (t.IsPrimitive || t.IsLayoutSequential)
                    return ExtractOperation.ValueStructureToBytes(objvalue);              
                if (objvalue is DateTime)
                    return ((DateTime)objvalue).ToBinary().GetBytes();
                if (objvalue is Enum)
                    return Convert.ToInt32(objvalue).GetBytes();
                return objvalue.GetSequentialBytes();
            }
            if (t.IsLayoutSequential)
                return objvalue.GetSequentialBytes();
            if (objvalue is IUnique)
                return ((IUnique)objvalue).GetBytes();         
            if (objvalue is String || objvalue is IFormattable)
                return objvalue.ToString().GetBytes();
            return new byte[0];
        }

        public unsafe static bool TryGetBytes(this Object objvalue, out Byte[] bytes)
        {
            if ((bytes = objvalue.GetBytes()).Length < 1)
                return false;
            return true;
        }

        public unsafe static Byte[] GetBytes(this IList objvalue)
        {
            int l = objvalue.Count;
            long offset = 0;
            byte[] b = new byte[512];
            fixed (byte* pb = &b[0])
            {
                for (int i = 0; i < l; i++)
                {
                    long s = 0;
                    object o = objvalue[i];
                    if (o is string)
                    {
                        s = ((string)o).Length *sizeof(char);
                        if ((s + offset) > b.Length)
                            Array.Resize(ref b, (int)(s + offset));

                        fixed (char* c = (string)o)
                            Extractor.Cpblk(pb, (uint)offset, (byte*)c, 0, (uint)s);
                    }
                    else
                    {
                        s = o.GetSize();
                        Extractor.StructureToPointer(o, pb + offset);
                    }
                    offset += s;
                }
            }
            return b;
        }

        public unsafe static bool TryGetBytes(this IList objvalue, out byte[] bytes)
        {
            int l = objvalue.Count;
            int offset = 0;
            bytes = null;
            byte[] b = new byte[(int)objvalue.GetSize()];
                for (int i = 0; i < l; i++)
                {
                    byte[] abyt = null;
                    if (objvalue[i].TryGetBytes(out abyt))
                    {
                        abyt.CopyBlock(b, (uint)offset, (uint)abyt.Length);
                        offset += abyt.Length;
                    }
                    else
                        return false;
                }
            bytes = b;
            return true;
        }

        public unsafe static Byte[] GetBytes(this String objvalue)
        {
            int l = objvalue.Length * sizeof(char);
            byte[] b = new byte[l];
            fixed (char* c = objvalue)
                fixed(byte* pb = b)
            {
                Extractor.Cpblk(pb, (byte*)c, (uint)l);
            }
            return b;
        }

        public unsafe static bool StructureEqual(this object structure, object other)
        {
            long asize = Extractor.GetSize(structure);
            long bsize = Extractor.GetSize(structure);
            if (asize < bsize)
                return false;
            byte* a = (byte*)structure.GetStructurePointer(), b = (byte*)other.GetStructurePointer();
            bool equal = Extractor.BlockEqual(a, 0, b, 0, asize);
            Marshal.FreeHGlobal(new IntPtr(a));
            Marshal.FreeHGlobal(new IntPtr(b));
            return equal;
        }
        public unsafe static bool CompareBlocks(byte* source, long srcOffset, byte* dest, long destOffset, long count)
        {
            long sl = count;
            long sl64 = sl / 8;
            long sl8 = sl % 8;
            long* lsrc = (long*)(source + srcOffset), ldst = (long*)(dest + destOffset);
            for (int i = 0; i < sl64; i++)
                if (*(&lsrc[i]) != (*(&ldst[i])))
                    return false;
            byte* psrc8 = source + (sl64 * 8), pdst8 = dest + (sl64 * 8);
            for (int i = 0; i < sl8; i++)
                if (*(&psrc8[i]) != (*(&pdst8[i])))
                    return false;
            return true;
        }

       
    }

}
