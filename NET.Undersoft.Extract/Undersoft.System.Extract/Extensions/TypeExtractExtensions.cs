
namespace System.Extract
{
    public static class TypeExtractExtenstion
    {
        public static object NewStructure(this Type structure, byte[] binary, long offset = 0)
        {
            //return _copier.PtrToStruct(binary, structure);

           // object o = Activator.CreateInstance(structure);
           return Extractor.BytesToStructure(binary, structure, offset);
          //  return o;
        }

        public unsafe static object NewStructure(this Type structure, byte* binary, long offset = 0)
        {
            // return _copier.PtrToStruct(binary, structure);
            // object o = Activator.CreateInstance(structure);
            return Extractor.PointerToStructure(binary, structure, offset);
            // return o;
        }

    }

}
