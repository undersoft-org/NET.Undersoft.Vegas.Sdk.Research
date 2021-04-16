
namespace System.Extract
{
    public static class IntPtrExtractExtenstion
    { 
        public static unsafe void CopyBlock(this IntPtr src, IntPtr dest, uint count)
        {
            Extractor.CopyBlock(dest, 0, src, 0, count);
        }
        public static unsafe void CopyBlock(this IntPtr src, IntPtr dest, uint offset, uint count)
        {
            Extractor.CopyBlock(dest, offset, src, 0, count);
        }
        public static unsafe void CopyBlock(this IntPtr src, uint srcOffset, IntPtr dest, uint destOffset, uint count)
        {
            Extractor.CopyBlock(dest, destOffset, src, srcOffset, count);
        }

        public static unsafe void CopyBlock(this IntPtr src, IntPtr dest, ulong count)
        {
            Extractor.CopyBlock(dest, 0, src, 0, count);
        }
        public static unsafe void CopyBlock(this IntPtr src, IntPtr dest, ulong offset, ulong count)
        {
            Extractor.CopyBlock(dest, offset, src, 0, count);
        }     
        public static unsafe void CopyBlock(this IntPtr src, ulong srcOffset, IntPtr dest, ulong destOffset, ulong count)
        {
            Extractor.CopyBlock(dest, destOffset, src, srcOffset, count);
        }

        public unsafe static object NewStructure(this IntPtr binary, Type structure, int offset)
        {
            return Extractor.PointerToStructure(binary, structure, offset);
        }

        public unsafe static object ToStructure(this IntPtr binary, object structure)
        {
            structure = Extractor.PointerToStructure(binary, structure);
            return structure;
        }

        public unsafe static void FromStructure(this IntPtr binary, object structure)
        {
            Extractor.StructureToPointer(structure, binary);
        }
    }

}
