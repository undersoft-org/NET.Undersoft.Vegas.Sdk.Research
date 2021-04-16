using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

namespace System.Extract
{
    public static partial class ExtractOperation
    {
        public static void CompileValueObjectToPointer(TypeBuilder tb)
        {
            var structToPtrMethod = tb.DefineMethod("ValueStructureToPointer",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(object), typeof(byte*), typeof(ulong) });
            var code = structToPtrMethod.GetILGenerator();


            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //byte* pserial = (byte*)Marshal.AllocHGlobal(size);
            //MemoryCopier.Copy(bstruct, pserial, 0, size); <- CpBlk
            //ptr = pserial;

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            //code.DeclareLocal(typeof(byte*));           //[3] [uint8*] //pserial


            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_1); // ld ValueType structure
                                        // code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarga, 1);   //&structure;
                                            // code.Emit(OpCodes.Ldind_Ref);
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*
            //--->

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //code.Emit(OpCodes.Ldarg_2); //ptr_arg
            //code.Emit(OpCodes.Ldloc_0); //size
            //code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("AllocHGlobal", new Type[] { typeof(int) }), null); //return allocate IntPtr
            //code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null);   //IntPtr (Alloc) => void*
            //code.Emit(OpCodes.Stloc_3);

            //MemoryCopier
            code.Emit(OpCodes.Ldarg_2); //dest: pserial
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Add);
            code.Emit(OpCodes.Ldloc_2); //source: pstructure
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            // A address
            // V value            
            //Stind: A <- V (A stores the pointer to V) 
            //ref ptr = pserial
            //code.Emit(OpCodes.Ldarg_2); // ptr 
            //code.Emit(OpCodes.Ldloc_3); // pserial
            //code.Emit(OpCodes.Stind_I); // into (ref byte* ptr) => address of pserial, ptr refer now to new allocation pserial

            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(structToPtrMethod, typeof(IExtractOperation).GetMethod("ValueStructureToPointer", new Type[] { typeof(object), typeof(byte*), typeof(ulong) }));
        }
        public static void CompileValueObjectToByteArrayRef(TypeBuilder tb)
        {
            var structToPtrMethod = tb.DefineMethod("ValueStructureToBytes",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(object), typeof(byte[]).MakeByRefType(), typeof(ulong) });
            var code = structToPtrMethod.GetILGenerator();


            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //byte* pserial = (byte*)Marshal.AllocHGlobal(size);
            //MemoryCopier.Copy(bstruct, pserial, 0, size); <- CpBlk
            //ptr = pserial;

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            //code.DeclareLocal(typeof(byte[]));           //[3] [uint8*] //pserial
            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);           //[3] [uint8*] //pserial


            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_1); // ld ValueType structure
            //code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarga, 1);   //&structure;
            code.Emit(OpCodes.Ldind_Ref);
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*
            //--->

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //code.Emit(OpCodes.Ldarg_2); //ptr_arg
            //code.Emit(OpCodes.Ldloc_0); //size
            //code.Emit(OpCodes.Newarr, typeof(byte));
            //code.Emit(OpCodes.Stloc_3);

            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldind_I);
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc_3);
            code.Emit(OpCodes.Ldloc_3); //dest: pserial
            code.Emit(OpCodes.Ldloc_2); //source: pstructure
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            // A address
            // V value            
            //Stind: A <- V (A stores the pointer to V) 
            //ref ptr = pserial
            //code.Emit(OpCodes.Ldarg_2); // ptr 
            //code.Emit(OpCodes.Ldloc_3); // pserial
            //code.Emit(OpCodes.Stind_I); // into (ref byte* ptr) => address of pserial, ptr refer now to new allocation pserial          

            code.Emit(OpCodes.Ret); // return   
            MethodInfo t = typeof(IExtractOperation).GetMethod("ValueStructureToBytes", new Type[] { typeof(object), typeof(byte[]).MakeByRefType(), typeof(ulong) });
            tb.DefineMethodOverride(structToPtrMethod, typeof(IExtractOperation).GetMethod("ValueStructureToBytes", new Type[] { typeof(object), typeof(byte[]).MakeByRefType(), typeof(ulong) }));
        }

        public static void CompileValueObjectToNewByteArray(TypeBuilder tb)
        {
            var structToPtrMethod = tb.DefineMethod("ValueStructureToBytes",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(byte[]),
                                             new Type[] { typeof(object) });
            var code = structToPtrMethod.GetILGenerator();


            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //byte* pserial = (byte*)Marshal.AllocHGlobal(size);
            //MemoryCopier.Copy(bstruct, pserial, 0, size); <- CpBlk
            //ptr = pserial;

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            code.DeclareLocal(typeof(byte[]));           //[3] [uint8*] //pserial
            code.DeclareLocal(typeof(byte[]).MakePointerType(), true);           //[3] [uint8*] //pserial


            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_1); // ld ValueType structure
                                        // code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarga, 1);   //&structure;
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*
            //--->

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //code.Emit(OpCodes.Ldarg_2); //ptr_arg
            code.Emit(OpCodes.Ldloc_0); //size
            code.Emit(OpCodes.Newarr, typeof(byte));
            code.Emit(OpCodes.Stloc_3);

            //MemoryCopier
            code.Emit(OpCodes.Ldloc_3); //dest: pserial
            code.Emit(OpCodes.Ldc_I4_0);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc, 4);
            code.Emit(OpCodes.Ldloc, 4);
            code.Emit(OpCodes.Ldloc_2); //source: pstructure
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            // A address
            // V value            
            //Stind: A <- V (A stores the pointer to V) 
            //ref ptr = pserial
            //code.Emit(OpCodes.Ldarg_2); // ptr 
            //code.Emit(OpCodes.Ldloc_3); // pserial
            //code.Emit(OpCodes.Stind_I); // into (ref byte* ptr) => address of pserial, ptr refer now to new allocation pserial

            code.Emit(OpCodes.Ldloc_3); // pserial
            code.Emit(OpCodes.Ret); // return   
            MethodInfo t = typeof(IExtractOperation).GetMethod("ValueStructureToBytes", new Type[] { typeof(object) });
            tb.DefineMethodOverride(structToPtrMethod, typeof(IExtractOperation).GetMethod("ValueStructureToBytes", new Type[] { typeof(object) }));
        }
        public static void CompileValueTypeToNewByteArray(TypeBuilder tb)
        {
            var structToPtrMethod = tb.DefineMethod("ValueStructureToBytes",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(byte[]),
                                             new Type[] { typeof(ValueType) });
            var code = structToPtrMethod.GetILGenerator();


            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //byte* pserial = (byte*)Marshal.AllocHGlobal(size);
            //MemoryCopier.Copy(bstruct, pserial, 0, size); <- CpBlk
            //ptr = pserial;

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            code.DeclareLocal(typeof(byte[]));           //[3] [uint8*] //pserial
            code.DeclareLocal(typeof(byte[]).MakePointerType(), true);           //[3] [uint8*] //pserial


            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_1); // ld ValueType structure
                                        //  code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarga, 1);   //&structure;
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*
            //--->

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //code.Emit(OpCodes.Ldarg_2); //ptr_arg
            code.Emit(OpCodes.Ldloc_0); //size
            code.Emit(OpCodes.Newarr, typeof(byte));
            code.Emit(OpCodes.Stloc_3);

            //MemoryCopier
            code.Emit(OpCodes.Ldloc_3); //dest: pserial
            code.Emit(OpCodes.Ldc_I4_0);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc, 4);
            code.Emit(OpCodes.Ldloc, 4);
            code.Emit(OpCodes.Ldloc_2); //source: pstructure
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            // A address
            // V value            
            //Stind: A <- V (A stores the pointer to V) 
            //ref ptr = pserial
            //code.Emit(OpCodes.Ldarg_2); // ptr 
            //code.Emit(OpCodes.Ldloc_3); // pserial
            //code.Emit(OpCodes.Stind_I); // into (ref byte* ptr) => address of pserial, ptr refer now to new allocation pserial

            code.Emit(OpCodes.Ldc_I4_0);
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Stloc, 4);

            code.Emit(OpCodes.Ldloc_3); // pserial
            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(structToPtrMethod, typeof(IExtractOperation).GetMethod("ValueStructureToBytes", new Type[] { typeof(ValueType) }));
        }
        public static void CompileValueObjectToNewPointer(TypeBuilder tb)
        {
            var structToPtrMethod = tb.DefineMethod("ValueStructureToPointer",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(byte*),
                                             new Type[] { typeof(object) });
            var code = structToPtrMethod.GetILGenerator();


            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //byte* pserial = (byte*)Marshal.AllocHGlobal(size);
            //MemoryCopier.Copy(bstruct, pserial, 0, size); <- CpBlk
            //ptr = pserial;

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            code.DeclareLocal(typeof(byte*));           //[3] [uint8*] //pserial


            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_1); // ld ValueType structure
                                        // code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarga, 1);   //&structure;
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*
            //--->

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //code.Emit(OpCodes.Ldarg_2); //ptr_arg
            code.Emit(OpCodes.Ldloc_0); //size
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("AllocHGlobal", new Type[] { typeof(int) }), null); //return allocate IntPtr
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null);   //IntPtr (Alloc) => void*
            code.Emit(OpCodes.Stloc_3);

            //MemoryCopier
            code.Emit(OpCodes.Ldloc_3); //dest: pserial
            code.Emit(OpCodes.Ldloc_2); //source: pstructure
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            // A address
            // V value            
            //Stind: A <- V (A stores the pointer to V) 
            //ref ptr = pserial
            //code.Emit(OpCodes.Ldarg_2); // ptr 
            //code.Emit(OpCodes.Ldloc_3); // pserial
            //code.Emit(OpCodes.Stind_I); // into (ref byte* ptr) => address of pserial, ptr refer now to new allocation pserial

            code.Emit(OpCodes.Ldloc_3);
            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(structToPtrMethod, typeof(IExtractOperation).GetMethod("ValueStructureToPointer", new Type[] { typeof(object) }));
        }

        public static void CompilePointerToValueObject(TypeBuilder tb)
        {
            var ptrToValueStructureMethod = tb.DefineMethod("PointerToValueStructure",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte*), typeof(object).MakeByRefType(), typeof(ulong) });
            var code = ptrToValueStructureMethod.GetILGenerator();

            //int size = Marshal.SizeOf(structure.GetType());
            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //MemoryCopier.Copy(pserial, bstruct, 0, size);

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure

            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_2); // ld ValueType structure
            code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarg, 2);   //&structure;
            //code.Emit(OpCodes.Ldind_Ref);
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //CpyBlk
            code.Emit(OpCodes.Ldloc_2); //dest: pstructure
            code.Emit(OpCodes.Ldarg_1); //source: ld ptr
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Add);
            //code.Emit(OpCodes.Ldind_I); //source: ld to stack addr of ptr
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(ptrToValueStructureMethod, typeof(IExtractOperation).GetMethod("PointerToValueStructure",
                                                                                          new Type[]
                                                                                          {
                                                                                              typeof(byte*),
                                                                                              typeof(object).MakeByRefType(),
                                                                                              typeof(ulong)
                                                                                          }));
        }
        public static void CompilePointerToValueType(TypeBuilder tb)
        {
            var ptrToValueStructureMethod = tb.DefineMethod("PointerToValueStructure",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte*), typeof(ValueType).MakeByRefType(), typeof(ulong) });
            var code = ptrToValueStructureMethod.GetILGenerator();

            //int size = Marshal.SizeOf(structure.GetType());
            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //MemoryCopier.Copy(pserial, bstruct, 0, size);

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure

            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_2); // ld ValueType structure
            code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarg, 2);   //&structure;
            //code.Emit(OpCodes.Ldind_Ref);
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //CpyBlk
            code.Emit(OpCodes.Ldloc_2); //dest: pstructure
            code.Emit(OpCodes.Ldarg_1); //source: ld ptr
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Add);
            //code.Emit(OpCodes.Ldind_I); //source: ld to stack addr of ptr
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(ptrToValueStructureMethod, typeof(IExtractOperation).GetMethod("PointerToValueStructure",
                                                                                          new Type[]
                                                                                          {
                                                                                              typeof(byte*),
                                                                                              typeof(ValueType).MakeByRefType(),
                                                                                              typeof(ulong)
                                                                                          }));
        }
        public static void CompileByteArrayToValueObject(TypeBuilder tb)
        {
            var ptrToValueStructureMethod = tb.DefineMethod("BytesToValueStructure",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte[]), typeof(object).MakeByRefType(), typeof(ulong) });
            var code = ptrToValueStructureMethod.GetILGenerator();

            //int size = Marshal.SizeOf(structure.GetType());
            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //MemoryCopier.Copy(pserial, bstruct, 0, size);

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);

            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_2); // ld ValueType structure
            code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(object) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarg, 2);   //&structure;
                                           // code.Emit(OpCodes.Ldind_Ref);
            code.Emit(OpCodes.Mkrefany, typeof(object)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit")
                                                                              .Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure

            //CpyBlk
            code.Emit(OpCodes.Ldloc_2); //dest: pstructure
            code.Emit(OpCodes.Ldarg_1); //source: ld ptr
            //code.Emit(OpCodes.Ldind_I); //source: ld to stack addr of ptr
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc_3);
            code.Emit(OpCodes.Ldloc_3);
            code.Emit(OpCodes.Ldloc_0);  //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            code.Emit(OpCodes.Ret); // return   

            tb.DefineMethodOverride(ptrToValueStructureMethod, typeof(IExtractOperation).GetMethod("BytesToValueStructure",
                                                                                        new Type[]
                                                                                        {
                                                                                            typeof(byte[]),
                                                                                            typeof(object).MakeByRefType(),
                                                                                            typeof(ulong)
                                                                                        }));
        }
        public static void CompileByteArrayToValueType(TypeBuilder tb)
        {
            var ptrToValueStructureMethod = tb.DefineMethod("BytesToValueStructure",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte[]), typeof(ValueType).MakeByRefType(), typeof(ulong) });
            var code = ptrToValueStructureMethod.GetILGenerator();

            //int size = Marshal.SizeOf(structure.GetType());
            //TypedReference tr = __makeref(structure);
            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            //fixed(byte* pserial = &barray[0] {
            //MemoryCopier.Copy(pserial, bstruct, 0, size); }
            //

            code.DeclareLocal(typeof(int));             //[0] size of structure
            code.DeclareLocal(typeof(TypedReference));  //[1] reference to structure (tr)            
            code.DeclareLocal(typeof(byte*));           //[2] [uint8*] //pstructure
            //code.DeclareLocal(typeof(byte*));                           //[3] [uint8*] //pserial 
            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);  //[4] [uint8& pinned] // pinned &barray[0]

            //size of structure [Marshal.SizeOf(structure)
            code.Emit(OpCodes.Ldarg_2); // ld ValueType structure
            code.Emit(OpCodes.Ldind_Ref);
            code.EmitCall(OpCodes.Call, typeof(Marshal).GetMethod("SizeOf", new[] { typeof(ValueType) }), null); // size of structure
            code.Emit(OpCodes.Stloc_0); //size

            //byte* bstruct = (byte*)*((IntPtr*)&tr);
            code.Emit(OpCodes.Ldarg, 2);   //&structure;
            code.Emit(OpCodes.Mkrefany, typeof(ValueType)); //TypedReference tr = __makeref(structure);
            code.Emit(OpCodes.Stloc_1); //TypedReference
            code.Emit(OpCodes.Ldloca, 1); // tr => &tr
            code.Emit(OpCodes.Conv_U); // (&tr) => (IntPtr)
            code.Emit(OpCodes.Ldind_I); // (IntPtr) => IntPtr*

            MethodInfo method_IntPtr_op_Explicit = typeof(IntPtr).GetMethods().Where(m => m.Name == "op_Explicit").Where(m => m.ReturnType == typeof(void*)).FirstOrDefault();
            code.EmitCall(OpCodes.Call, method_IntPtr_op_Explicit, null); // (IntPtr*) => *(IntPtr*)
            code.Emit(OpCodes.Stloc, 2);    //*(IntPtr) => byte* pstructure


            //fixed (byte* parray = &array[0])

            code.Emit(OpCodes.Ldarg_1); //load barray, which stores the address to array            
            code.Emit(OpCodes.Ldarg_3);  //x=0
            code.Emit(OpCodes.Ldelema, typeof(byte)); //get address of the x element of array, i.e., &(barray[x=0])
            code.Emit(OpCodes.Stloc, 3); //pinned (&array[0]), i.e., a pointer-address as a pinned array

            //convert pinned (&barray[x=0) => *parray
            //code.Emit(OpCodes.Ldloc, 4); // 
            //code.Emit(OpCodes.Conv_U); //
            //code.Emit(OpCodes.Stloc_3); // parray = (&array[x=0])

            //Copy Blk
            code.Emit(OpCodes.Ldloc_2); //dest: *pstructure
            code.Emit(OpCodes.Ldloc_3); //source: *parray
            code.Emit(OpCodes.Ldloc_0); //size: 
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);

            //clear pinned?
            //code.Emit(OpCodes.Ldc_I4_0);
            //code.Emit(OpCodes.Conv_U);
            //code.Emit(OpCodes.Stloc, 3);
            code.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(ptrToValueStructureMethod, typeof(IExtractOperation).GetMethod("BytesToValueStructure", new Type[] { typeof(byte[]), typeof(ValueType).MakeByRefType(), typeof(ulong) }));
        }
    }
}