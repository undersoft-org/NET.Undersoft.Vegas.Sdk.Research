using System.Runtime.InteropServices;
using System.Collections;
using System.Linq;

namespace System.Extract
{
    public static partial class Extractor
    {
        public static unsafe void CopyBlock(byte* dest, byte* src, int count)
        {
            ExtractOperation.CopyBlock(dest, 0, src, 0, (uint)count);
        }
        public static unsafe void CopyBlock(void* dest, void* src, int count)
        {
            ExtractOperation.CopyBlock((byte*)dest, 0, (byte*)src, 0, (uint)count);
        }

        public static unsafe void CopyBlock(byte* dest, byte* src, int destOffset, int count)
        {
            ExtractOperation.CopyBlock(dest, (uint)destOffset, src, 0, (uint)count);
        }
        public static unsafe void CopyBlock(void* dest, void* src, int destOffset, int count)
        {
            ExtractOperation.CopyBlock((byte*)dest, (uint)destOffset, (byte*)src, 0, (uint)count);
        }

        public static unsafe void CopyBlock(byte* dest, int destOffset, byte* src, int srcOffset, int count)
        {
            ExtractOperation.CopyBlock(dest, (uint)destOffset, src, (uint)srcOffset, (uint)count);
        }
        public static unsafe void CopyBlock(void* dest, int destOffset, void* src, int srcOffset, int count)
        {
            ExtractOperation.CopyBlock((byte*)dest, (uint)destOffset, (byte*)src, (uint)srcOffset, (uint)count);
        }

        public static unsafe void CopyBlock(byte* dest, byte* src, long count)
        {
            ExtractOperation.CopyBlock(dest, 0, src, 0, (ulong)count);
        }
        public static unsafe void CopyBlock(void* dest, void* src, long count)
        {
            ExtractOperation.CopyBlock((byte*)dest, 0, (byte*)src, 0, (ulong)count);
        }

        public static unsafe void CopyBlock(byte* dest, byte* src, long destOffset, long count)
        {
            ExtractOperation.CopyBlock(dest, (ulong)destOffset, src, 0, (ulong)count);
        }
        public static unsafe void CopyBlock(void* dest, void* src, long destOffset, long count)
        {
            ExtractOperation.CopyBlock((byte*)dest, (ulong)destOffset, (byte*)src, 0, (ulong)count);
        }

        public static unsafe void CopyBlock(byte* dest, long destOffset, byte* src, long srcOffset, long count)
        {
            ExtractOperation.CopyBlock(dest, (ulong)destOffset, src, (ulong)srcOffset, (ulong)count);
        }
        public static unsafe void CopyBlock(void* dest, long destOffset, void* src, long srcOffset, long count)
        {
            ExtractOperation.CopyBlock((byte*)dest, (ulong)destOffset, (byte*)src, (ulong)srcOffset, (ulong)count);
        }

        public static unsafe void CopyBlock(byte[] dest, byte[] src, int count)
        {
            ExtractOperation.CopyBlock(dest, 0, src, 0, (uint)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, IntPtr src, int count)
        {
            ExtractOperation.CopyBlock((byte*)(dest.ToPointer()), 0, (byte*)(src.ToPointer()), 0, (uint)count);
        }

        public static unsafe void CopyBlock(byte[] dest, byte[] src, int destOffset, int count)
        {
            ExtractOperation.CopyBlock(dest, (uint)destOffset, src, 0, (uint)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, IntPtr src, int destOffset, int count)
        {
            ExtractOperation.CopyBlock((byte*)src.ToPointer(), (uint)destOffset, (byte*)dest.ToPointer(), 0, (uint)count);
        }

        public static unsafe void CopyBlock(byte[] dest, int destOffset, byte[] src, int srcOffset, int count)
        {
            ExtractOperation.CopyBlock(dest, (uint)destOffset, src, (uint)srcOffset, (uint)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, int destOffset, IntPtr src, int srcOffset, int count)
        {
            ExtractOperation.CopyBlock((byte*)(dest.ToPointer()), (uint)destOffset, (byte*)(src.ToPointer()), (uint)srcOffset, (uint)count);
        }

        public static unsafe void CopyBlock(byte[] dest, byte[] src, long count)
        {
            ExtractOperation.CopyBlock(dest, 0, src, 0, (ulong)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, IntPtr src, long count)
        {
            ExtractOperation.CopyBlock((byte*)(dest.ToPointer()), 0, (byte*)(src.ToPointer()), 0, (ulong)count);
        }

        public static unsafe void CopyBlock(byte[] dest, byte[] src, long destOffset, long count)
        {
            ExtractOperation.CopyBlock(dest, (ulong)destOffset, src, 0, (ulong)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, IntPtr src, long destOffset, long count)
        {
            ExtractOperation.CopyBlock((byte*)(dest.ToPointer()), (ulong)destOffset, (byte*)(src.ToPointer()), 0, (ulong)count);
        }

        public static unsafe void CopyBlock(byte[] dest, long destOffset, byte[] src, long srcOffset, long count)
        {
            ExtractOperation.CopyBlock(dest, (ulong)destOffset, src, (ulong)srcOffset, (ulong)count);
        }
        public static unsafe void CopyBlock(IntPtr dest, long destOffset, IntPtr src, long srcOffset, long count)
        {
            ExtractOperation.CopyBlock((byte*)(dest.ToPointer()), (ulong)destOffset, (byte*)(src.ToPointer()), (ulong)srcOffset, (ulong)count);
        }

        public unsafe static bool BlockEqual(byte[] source, byte[] dest)
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
        public unsafe static bool BlockEqual(byte* source, long srcOffset, byte* dest, long destOffset, long count)
        {
            long sl = count;
            long sl64 = sl / 8;
            long sl8 = sl % 8;
            long* lsrc = (long*)(source + srcOffset), ldst = (long*)(dest + destOffset);
            for (int i = 0; i < sl64; i++)
                if (*(&lsrc[i]) != *(&ldst[i]))
                    return false;
            byte* psrc8 = source + (sl64 * 8), pdst8 = dest + (sl64 * 8);
            for (int i = 0; i < sl8; i++)
                if (*(&psrc8[i]) != *(&pdst8[i]))
                    return false;
            return true;
        }

        public static unsafe object BytesToStructure(byte[] binary, Type structure, long offset)
        {
            fixed (byte* b = &binary[offset])
                return PointerToStructure(new IntPtr(b), structure, 0);
        }
        public unsafe static object PointerToStructure(byte* binary, Type structure, long offset)
        {
            return PointerToStructure(new IntPtr(binary + offset), structure, 0);
        }
        public unsafe static object PointerToStructure(IntPtr binary, Type structure, int offset)
        {
            if (structure == typeof(DateTime))
                return DateTime.FromBinary((long)Marshal.PtrToStructure(binary + (int)offset, typeof(long)));
            else
                return Marshal.PtrToStructure(binary, structure);
        }

        public unsafe static object PointerToStructure(byte* binary, object structure)
        {
           return PointerToStructure(new IntPtr(binary), structure);
        }
        public unsafe static object PointerToStructure(IntPtr binary, object structure)
        {
            if (structure is ValueType)
            {
                Type t = structure.GetType();
                if (t.IsPrimitive || t.IsLayoutSequential)
                    structure = ExtractOperation.PointerToValueStructure((byte*)binary, structure, 0);
                else
                    structure = PointerToStructure(binary, structure.GetType(), 0);
            }
            else
                Marshal.PtrToStructure(binary, structure);
            return structure;
        }

        public unsafe static ValueType PointerToStructure(byte* binary, ValueType structure)
        {
           return ExtractOperation.PointerToValueStructure(binary, structure, 0);
        }
        public unsafe static ValueType PointerToStructure(IntPtr binary, ValueType structure)
        {
           return ExtractOperation.PointerToValueStructure((byte*)binary, structure, 0);
        }

        public unsafe static object BytesToStructure(byte[] binary, object structure, long offset)
        {
            if (structure is ValueType)
              return ExtractOperation.BytesToValueStructure(binary, structure, 0);
            else
            {
                fixed (byte* b = &binary[offset])
                   return PointerToStructure(new IntPtr(b), structure);
            }
        }
        public unsafe static ValueType BytesToStructure(byte[] binary, ValueType structure, long offset)
        {
           return ExtractOperation.BytesToValueStructure(binary, structure, 0);
        }

        public unsafe static void StructureToPointer(object structure, byte* binary)
        {
            IntPtr p = new IntPtr(binary);
            StructureToPointer(structure, p);
            binary = (byte*)p;
        }
        public unsafe static void StructureToPointer(object structure, IntPtr binary)
        {
            if (structure is ValueType)
            {
                Type t = structure.GetType();
                if (t.IsPrimitive || t.IsLayoutSequential)
                {                   
                    Marshal.StructureToPtr(structure, binary, false);
                    return;
                }

                if (structure is DateTime)
                {
                    structure = ((DateTime)structure).ToBinary();
                    Marshal.StructureToPtr(structure, binary, false);
                    return;
                }

                if (t.IsLayoutSequential)
                {
                    ExtractOperation.ValueStructureToPointer(structure, (byte*)binary, 0);
                    return;
                }

            }

            Marshal.StructureToPtr(structure, binary, false);
        }
        public unsafe static void StructureToBytes(object structure, ref byte[] binary, long offset)
        {
            fixed (byte* pb = &binary[offset])
            {
                IntPtr p = new IntPtr(pb);
                StructureToPointer(structure, p);
            }
        }

        public unsafe static void StructureToPointer(ValueType structure, byte* binary)
        {
            IntPtr p = new IntPtr(binary);
            StructureToPointer(structure, p);
            binary = (byte*)p;
        }
        public unsafe static void StructureToPointer(ValueType structure, IntPtr binary)
        {
            if (structure is DateTime)           
                structure = ((DateTime)structure).ToBinary();
            if (structure.GetType().IsLayoutSequential)
            {
                ExtractOperation.ValueStructureToPointer(structure, (byte*)binary, 0);
                return;
            }

            Marshal.StructureToPtr(structure, binary, false);
        }
        public unsafe static void StructureToBytes(ValueType structure, ref byte[] binary, long offset)
        {
            fixed (byte* pb = &binary[offset])
            {
                IntPtr p = new IntPtr(pb);
                StructureToPointer(structure, p);
            }
        }

        public unsafe static byte[] GetStructureBytes(object structure)
        {
            byte[] b = null;
            object _structure = structure;
            if (_structure is string)
            {
                int l = ((string)_structure).Length;
                b = new byte[l];
                fixed (char* c = (string)_structure)
                fixed (byte* pb = b)
                    CopyBlock(pb, (byte*)c, l);
            }

            if (structure is ValueType)
            {
                Type t = structure.GetType();
                if (t.IsPrimitive || t.IsLayoutSequential)
                    return ExtractOperation.ValueStructureToBytes(structure);

                if (structure is DateTime)
                {
                    b = new byte[8];
                    _structure = ((DateTime)_structure).ToBinary();
                }
                else if (structure is Enum)
                {
                    b = new byte[4];
                    structure = Convert.ToInt32((Enum)structure);
                }
                else
                {
                    b = new byte[Marshal.SizeOf(_structure)];
                }
            }
            else
                b = new byte[Marshal.SizeOf(_structure)];

          
            fixed (byte* pb = b)
                Marshal.StructureToPtr(_structure, new IntPtr(pb), false);
            return b;
        }
        public unsafe static byte[] GetStructureBytes(ValueType structure)
        {
          
            Type t = structure.GetType();
            if (t.IsPrimitive || t.IsLayoutSequential)
                return ExtractOperation.ValueStructureToBytes(structure);

            byte[] b = null;
            var _structure = structure;
            if (structure is DateTime)
            {
                b = new byte[8];
                _structure = ((DateTime)structure).ToBinary();
            }
            else if (structure is Enum)
            {
                b = new byte[4];
                _structure = Convert.ToInt32((Enum)structure);
            }
            else
            {
                b = new byte[Marshal.SizeOf(_structure)];
            }


            fixed (byte* pb = b)
                Marshal.StructureToPtr(_structure, new IntPtr(pb), false);
            return b;
        }
        public unsafe static byte* GetStructurePointer(object structure)
        {
            int size = 0;

            if (structure is ValueType)
            {
                Type t = structure.GetType();
                if (t.IsPrimitive || t.IsLayoutSequential)
                    return ExtractOperation.ValueStructureToPointer(structure);

                if (structure is DateTime)
                {
                    size = 8;
                    structure = ((DateTime)structure).ToBinary();
                }
                else if (structure is ISerialNumber)
                    size = 24;
                else if (structure is IUnique)
                    size = 8;
                else if (structure is Enum)
                {
                    size = 4;
                    structure = Convert.ToInt32((Enum)structure);
                }
                else
                    size = Marshal.SizeOf(structure);
            }
            else
                size = Marshal.SizeOf(structure);

            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, p, false);

            return (byte*)p;
        }
        public unsafe static IntPtr GetStructureIntPtr(object structure)
        {
            int size = 0;
         
            if (structure is ValueType)
            {
                Type t = structure.GetType();
                if (t.IsPrimitive || t.IsLayoutSequential)
                    return ExtractOperation.ValueStructureToIntPtr(structure);

                if (structure is DateTime)
                {
                    size = 8;
                    structure = ((DateTime)structure).ToBinary();
                }             
                else if (structure is ISerialNumber)
                    size = 24;
                else if (structure is IUnique)
                    size = 8;
                else if (structure is Enum)
                {
                    size = 4;
                    structure = Convert.ToInt32((Enum)structure);
                }
                else
                    size = Marshal.SizeOf(structure);
            }
            else
                size = Marshal.SizeOf(structure);

            IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
            Marshal.StructureToPtr(structure, p, false);

            return p;
        }
    
        private static long ValueTypeElementSize(Type e)
        {
            if (e.IsPrimitive || e.IsLayoutSequential)
                return Marshal.SizeOf(e);
            if (e == typeof(DateTime))
                return 8;
            if (e == typeof(Enum))
                return 4;
            if (e == typeof(ISerialNumber))
                return 24;
            if (e == typeof(IUnique))
                return 4;           
            return Marshal.SizeOf(e);
        }
        private static long ValueTypeObjectSize(object structure)
        {
            Type t = structure.GetType();
            if (t.IsPrimitive || t.IsLayoutSequential)
                return Marshal.SizeOf(structure);
            if (structure is DateTime)
                return 8;
            if (structure is Enum)
                return 4;
            if (structure is ISerialNumber)
                return 24;
            if (structure is IUnique)
                return 8;            
            return Marshal.SizeOf(structure);
        }

        public unsafe static long[] GetSizes(object[] array)
        {
            return GetSizes((IList)array);           
        }      
        public unsafe static long[] GetSizes(IList list)
        {
            long c = list.Count;
            if (c > 0)
            {
                if (list.GetType().HasElementType)
                {
                    var e = list.GetType().GetElementType();
                    if (e.IsValueType)
                        return new long[] { ValueTypeElementSize(e) * c };
                    if (e == typeof(string))
                        return list.Cast<string>().Select(p => (long)p.Length).ToArray();
                    if (e.IsLayoutSequential)
                        return new long[c].Select(l => l = Marshal.SizeOf(e)).ToArray();
                    if(e.IsArray)
                        return GetSizes(list[0]);
                }
                return list.Cast<object>()
                                .Select(o => o.GetSize()).ToArray();
            }
            return new long[0];
        }
        public unsafe static long[] GetSizes(object structure)
        {
            if (structure is ValueType)
                return new long[] { ValueTypeObjectSize(structure) };             
            if (structure is String || structure is IFormattable)
                return new long[] { structure.ToString().Length };
            if (structure.GetType().IsLayoutSequential)
                return new long[] { Marshal.SizeOf(structure) };
            if (structure is IList)
                return GetSizes(((IList)structure));
            
            return new long[0];
        }
        public unsafe static long GetSize(object structure)
        {          
            if (structure is ValueType)            
               return ValueTypeObjectSize(structure);
            if (structure.GetType().IsLayoutSequential)
                return (long)Marshal.SizeOf(structure);
            if (structure is String || structure is IFormattable)
                return structure.ToString().Length * sizeof(char);
            if (structure is IList)
                return GetSizes(((IList)structure)).Sum();
            return 0;
        }
       

    }
}
