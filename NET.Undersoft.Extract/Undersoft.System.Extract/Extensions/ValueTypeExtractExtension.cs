using System.Runtime.InteropServices;

namespace System.Extract
{
    public static class ValueTypeExtractExtenstion
    {

        public unsafe static void StructureFrom(this ValueType structure, byte* binary)
        {
            structure = Extractor.PointerToStructure(binary, structure);
        }
        public unsafe static void StructureFrom(this ValueType structure, IntPtr binary)
        {
            structure = Extractor.PointerToStructure(binary, structure);
        }
        public unsafe static void StructureFrom(this ValueType structure, byte[] binary, long offset = 0)
        {
            structure = Extractor.BytesToStructure(binary, structure, offset);
        }

        public unsafe static Byte[] GetBytes(this ValueType objvalue)
        {
            return Extractor.GetStructureBytes(objvalue);
        }


        //public unsafe static Byte[] GetBytes(this ValueType objvalue)
        //{
        //    if (objvalue.GetType().BaseType.IsPrimitive)
        //    {
        //        byte[] b = new byte[Marshal.SizeOf(objvalue)];
        //        fixed (byte* pb = b)
        //            Marshal.StructureToPtr(objvalue, new IntPtr(pb), true);
        //        return b;
        //    }
        //    if (objvalue is IUnique)
        //        return ((IUnique)objvalue).GetBytes();
        //    else if (objvalue is DateTime)
        //        return ((DateTime)objvalue).ToBinary().GetStructureBytes();
        //    else if (objvalue is Enum)
        //        return objvalue.ToString().GetBytes();
        //    else
        //        return objvalue.GetStructureBytes();
        //}

        public unsafe static byte[] GetBytes(this ulong hash)
        {
            byte[] q = new byte[8];
            fixed (byte* pbyte = q)
                *((ulong*)pbyte) = hash;
            return q;
        }
        public unsafe static byte[] GetBytes(this long hash)
        {
            byte[] q = new byte[8];
            fixed (byte* pbyte = q)
                *((long*)pbyte) = hash;
            return q;
        }
        public unsafe static byte[] GetBytes(this uint hash)
        {
            byte[] q = new byte[4];
            fixed (byte* pbyte = q)
                *((uint*)pbyte) = hash;
            return q;
        }
        public unsafe static byte[] GetBytes(this int hash)
        {
            byte[] q = new byte[4];
            fixed (byte* pbyte = q)
                *((int*)pbyte) = hash;
            return q;
        }

        public unsafe static Byte[] GetPrimitiveBytes(this object objvalue)
        {
            byte[] b = new byte[Marshal.SizeOf(objvalue)];
            fixed (byte* pb = b)
                objvalue.StructureTo(pb);
            return b;
        }
        public unsafe static long GetPrimitiveLong(this object objvalue)
        {
            byte* ps = stackalloc byte[8];
            Marshal.StructureToPtr(objvalue, (IntPtr)ps, false);
            return *(long*)ps;
        }
    }

}
