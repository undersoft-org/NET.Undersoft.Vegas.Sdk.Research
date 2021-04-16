using System.Uniques;
using System.Linq;
using System.Multemic;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace System.Instants
{
    public class InstantSelectionCompiler
    {
        private readonly ConstructorInfo marshalAsCtor =
            typeof(MarshalAsAttribute)
            .GetConstructor(new Type[] { typeof(UnmanagedType) });
        private FieldBuilder selectiveField = null;
        private FieldBuilder multemicField = null;
        private InstantSelection selection;
        private IFigures table => selection.Collection;
        private Type SelectionType = typeof(FigureSelection);

        public InstantSelectionCompiler(InstantSelection instantSelection)
        {
            selection = instantSelection;
        }

        public Type CompileFigureType(string typeName)
        {
            TypeBuilder tb = GetTypeBuilder(typeName);

            CreateSelectionField(tb);

            CreateFiguresField(tb);

         //   CreateArrayLengthField(tb);         

            CreateElementByIntProperty(tb);

            CreateItemByIntProperty(tb);

            CreateItemByStringProperty(tb);

            return tb.CreateTypeInfo();
        }

        private TypeBuilder GetTypeBuilder(string typeName)
        {
            string typeSignature = (typeName != null && typeName != "") ? typeName : "InstantSelection_" + table.GetType().Name + DateTime.Now.ToString("yyyyMMddHHmmss");
            AssemblyName an = new AssemblyName(typeSignature);

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(typeSignature + "Module");
            TypeBuilder tb = null;

            tb = moduleBuilder.DefineType(typeSignature, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable |
                                                         TypeAttributes.AnsiClass);

            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(DataContractAttribute).GetConstructor(Type.EmptyTypes), new object[0]));
            tb.SetParent(typeof(FigureSelection));
            return tb;
        }

        private FieldBuilder CreateField(TypeBuilder tb, Type type, string name)
        {
            return tb.DefineField("_" + name, type, FieldAttributes.Private);
        }

        private void CreateSelectionField(TypeBuilder tb)
        {
            FieldBuilder fb = CreateField(tb, typeof(object).MakeArrayType(), "Selection");
            selectiveField = fb;
            //CreateMarshalAttribue(fb, new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = selective.Cou });
            //PropertyBuilder prop = tb.DefineProperty("Selection", PropertyAttributes.HasDefault,
            //                                         typeof(object).MakeArrayType(), new Type[] { typeof(object).MakeArrayType() });

            PropertyInfo iprop = SelectionType.GetProperty("Selection");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            tb.DefineMethodOverride(getter, accessor);

            //getter.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, fb); // load
            il.Emit(OpCodes.Ret); // return

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                               mutator.CallingConvention, mutator.ReturnType, argTypes);
            tb.DefineMethodOverride(setter, mutator);

            //prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Stfld, fb); // assign
            il.Emit(OpCodes.Ret);

        }

        private void CreateFiguresField(TypeBuilder tb)
        {
            FieldBuilder fb = CreateField(tb, typeof(object).MakeArrayType(), "Collection");
            multemicField = fb;
            //CreateMarshalAttribue(fb, new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = selective.Length });
            //PropertyBuilder prop = tb.DefineProperty("Selection", PropertyAttributes.HasDefault,
            //                                         typeof(object).MakeArrayType(), new Type[] { typeof(object).MakeArrayType() });

            PropertyInfo iprop = SelectionType.GetProperty("Collection");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            tb.DefineMethodOverride(getter, accessor);

            //getter.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, fb); // load
            il.Emit(OpCodes.Ret); // return

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                               mutator.CallingConvention, mutator.ReturnType, argTypes);
            tb.DefineMethodOverride(setter, mutator);

            //prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Stfld, fb); // assign
            il.Emit(OpCodes.Ret);

        }

        private void CreateNewSelectionObject(TypeBuilder tb)
        {
            MethodInfo createArray = SelectionType.GetMethod("NewSelection");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();
            il.DeclareLocal(typeof(IFigure).MakeArrayType());

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newarr, typeof(IFigure));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Stfld, selectiveField); //    
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
        }

        private void CreateArrayLengthField(TypeBuilder tb)
        {

            //PropertyBuilder prop = tb.DefineProperty("Length", PropertyAttributes.HasDefault,
            //                                         typeof(int), Type.EmptyTypes);

            PropertyInfo iprop = SelectionType.GetProperty("Length");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            tb.DefineMethodOverride(getter, accessor);

            //prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, selectiveField); // load
            il.Emit(OpCodes.Ldlen); // load
            il.Emit(OpCodes.Ret); // return

            //return prop;
        }
       
        private void CreateMarshalAttribue(FieldBuilder field, MarshalAsAttribute attrib)
        {
            List<object> attribValues = new List<object>(1);
            List<FieldInfo> attribFields = new List<FieldInfo>(1);
            attribValues.Add(attrib.SizeConst);
            attribFields.Add(attrib.GetType().GetField("SizeConst"));
            field.SetCustomAttribute(new CustomAttributeBuilder(marshalAsCtor, new object[] { attrib.Value }, attribFields.ToArray(), attribValues.ToArray()));
        }

        private void CreateElementByIntProperty(TypeBuilder tb)
        {

            PropertyInfo prop = typeof(FigureSelection).GetProperty("Item", new Type[] { typeof(int) });
            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                  accessor.CallingConvention, accessor.ReturnType, argTypes);
                tb.DefineMethodOverride(method, accessor);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("get_Item", new Type[] { typeof(int) }), null);
                il.Emit(OpCodes.Ret); // end
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                   mutator.CallingConvention, mutator.ReturnType, argTypes);
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.Emit(OpCodes.Ldarg_2); // value
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("set_Item", new Type[] { typeof(int), typeof(IFigure) }), null);
                il.Emit(OpCodes.Ret); // end
            }
        }

        private void CreateItemByIntProperty(TypeBuilder tb)
        {
            PropertyInfo prop = typeof(FigureSelection).GetProperty("Item", new Type[] { typeof(int), typeof(int) });
            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                  accessor.CallingConvention, accessor.ReturnType, argTypes);
                tb.DefineMethodOverride(method, accessor);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.Emit(OpCodes.Ldarg_2); // fieldid
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("get_Item", new Type[] { typeof(int), typeof(int) }), null);
                il.Emit(OpCodes.Ret); // end
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                   mutator.CallingConvention, mutator.ReturnType, argTypes);
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.Emit(OpCodes.Ldarg_2); // fieldid
                il.Emit(OpCodes.Ldarg_3); // value
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("set_Item", new Type[] { typeof(int), typeof(int), typeof(object) }), null);
                il.Emit(OpCodes.Ret); // end
            }   
        }

        private void CreateItemByStringProperty(TypeBuilder tb)
        {
            PropertyInfo prop = typeof(FigureSelection).GetProperty("Item", new Type[] { typeof(int), typeof(string) });
            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                   accessor.CallingConvention, accessor.ReturnType, argTypes);
                tb.DefineMethodOverride(method, accessor);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.Emit(OpCodes.Ldarg_2); // fieldid
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("get_Item", new Type[] { typeof(int), typeof(string) }), null);
                il.Emit(OpCodes.Ret); // end
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);
                MethodBuilder method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                   mutator.CallingConvention, mutator.ReturnType, argTypes);
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, selectiveField); // foo load   
                il.Emit(OpCodes.Ldarg_1); // rowid
                il.Emit(OpCodes.Ldarg_2); // fieldid
                il.Emit(OpCodes.Ldarg_3); // value
                il.EmitCall(OpCodes.Callvirt, typeof(IFigures).GetMethod("set_Item", new Type[] { typeof(int), typeof(string), typeof(object) }), null);
                il.Emit(OpCodes.Ret); // end
            }
        }

        private PropertyBuilder CreateProperty(TypeBuilder tb, FieldBuilder field, Type type, string name)
        {

            PropertyBuilder prop = tb.DefineProperty(name, PropertyAttributes.HasDefault,
                                                     type, new Type[] { type });

            MethodBuilder getter = tb.DefineMethod("get_" + name, MethodAttributes.Public |
                                                            MethodAttributes.HideBySig, type,
                                                            Type.EmptyTypes);
            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, field); // load
            il.Emit(OpCodes.Ret); // return

            MethodBuilder setter = tb.DefineMethod("set_" + name, MethodAttributes.Public |
                                                            MethodAttributes.HideBySig, typeof(void),
                                                            new Type[] { type });
            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Stfld, field); // assign
            il.Emit(OpCodes.Ret);

            return prop;

        }

    }


}
