namespace System.Extract
{   
    public interface IExtractOperation
    {
        void CopyBlock(byte[] source, uint srcOffset, byte[] dest, uint destOffset, uint count);
        unsafe void CopyBlock(byte* source, uint srcOffset, byte* dest, uint destOffset, uint count);

        void CopyBlock(byte[] source, ulong srcOffset, byte[] dest, ulong destOffset, ulong count);
        unsafe void CopyBlock(byte* source, ulong srcOffset, byte* dest, ulong destOffset, ulong count);      

        unsafe void ValueStructureToPointer(object structure, byte* ptr, ulong offset);
        void ValueStructureToBytes(object structure, ref byte[] ptr, ulong offset);

        byte[] ValueStructureToBytes(object structure);
        byte[] ValueStructureToBytes(ValueType structure);
        unsafe byte* ValueStructureToPointer(object structure);

        unsafe void PointerToValueStructure(byte* ptr, ref object structure, ulong offset);
        unsafe void PointerToValueStructure(byte* ptr, ref ValueType structure, ulong offset);

        unsafe void BytesToValueStructure(byte[] ptr, ref object structure, ulong offset);
        unsafe void BytesToValueStructure(byte[] array, ref ValueType structure, ulong offset);
    }
}
