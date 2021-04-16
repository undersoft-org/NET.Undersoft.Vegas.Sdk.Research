using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.Linq;

namespace System.Extract
{
    public static partial class ExtractOperation
    {
        public static void CompileCopyByteArrayBlockUInt32(TypeBuilder tb)
        {
            var copyMethod = tb.DefineMethod("CopyBlock",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte[]), typeof(uint), typeof(byte[]), typeof(uint), typeof(uint) });
            var code = copyMethod.GetILGenerator();

            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);
            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);

            //updated by Darek
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldelema, typeof(byte)); 
            code.Emit(OpCodes.Stloc_0);
            code.Emit(OpCodes.Ldloc_0);
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Ldarg, 4);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc_1);
            code.Emit(OpCodes.Ldloc_1);
            code.Emit(OpCodes.Ldarg, 5);
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);          
            code.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(copyMethod, typeof(IExtractOperation).GetMethod("CopyBlock", new Type[] { typeof(byte[]), typeof(uint), typeof(byte[]), typeof(uint), typeof(uint) }));
        }
        public static void CompileCopyPointerBlockUInt32(TypeBuilder tb)
        {
            var copyMethod = tb.DefineMethod("CopyBlock",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte*), typeof(uint), typeof(byte*), typeof(uint), typeof(uint) });
            var code = copyMethod.GetILGenerator();

            //updated by Darek
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Add);
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Ldarg, 4);
            code.Emit(OpCodes.Add);
            code.Emit(OpCodes.Ldarg, 5);
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);
            code.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(copyMethod, typeof(IExtractOperation).GetMethod("CopyBlock", new Type[] { typeof(byte*), typeof(uint), typeof(byte*), typeof(uint), typeof(uint) }));
        }

        public static void CompileCopyByteArrayBlockUInt64(TypeBuilder tb)
        {
            var copyMethod = tb.DefineMethod("CopyBlock",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte[]), typeof(ulong), typeof(byte[]), typeof(ulong), typeof(ulong) });
            var code = copyMethod.GetILGenerator();

            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);
            code.DeclareLocal(typeof(byte[]).MakePointerType(), pinned: true);

            //updated by Darek
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldelema, typeof(byte));  
            code.Emit(OpCodes.Stloc_0);
            code.Emit(OpCodes.Ldloc_0);
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Ldarg, 4);
            code.Emit(OpCodes.Ldelema, typeof(byte));
            code.Emit(OpCodes.Stloc_1);
            code.Emit(OpCodes.Ldloc_1);
            code.Emit(OpCodes.Ldarg, 5);
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);      
            code.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(copyMethod, typeof(IExtractOperation).GetMethod("CopyBlock", new Type[] { typeof(byte[]), typeof(ulong), typeof(byte[]), typeof(ulong), typeof(ulong) }));
        }
        public static void CompileCopyPointerBlockUInt64(TypeBuilder tb)
        {
            var copyMethod = tb.DefineMethod("CopyBlock",
                                             MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                                             typeof(void),
                                             new Type[] { typeof(byte*), typeof(ulong), typeof(byte*), typeof(ulong), typeof(ulong) });
            var code = copyMethod.GetILGenerator();

            //updated by Darek
            code.Emit(OpCodes.Ldarg_2);
            code.Emit(OpCodes.Ldarg_1);
            code.Emit(OpCodes.Ldarg_3);
            code.Emit(OpCodes.Add);
            code.Emit(OpCodes.Ldarg, 4);
            code.Emit(OpCodes.Conv_U);
            code.Emit(OpCodes.Cpblk);
            code.Emit(OpCodes.Ret);

            tb.DefineMethodOverride(copyMethod, typeof(IExtractOperation).GetMethod("CopyBlock", new Type[] { typeof(byte*), typeof(ulong), typeof(byte*), typeof(ulong), typeof(ulong) }));
        }

    }

}