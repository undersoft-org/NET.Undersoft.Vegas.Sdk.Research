using System.Uniques;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace System.Instants
{   
    public class InstantFigureValueTypeCompiler
    {
        private readonly ConstructorInfo dataMemberCtor = typeof(DataMemberAttribute)
                                                 .GetConstructor(Type.EmptyTypes);
        private readonly PropertyInfo[] dataMemberProps = new[] { typeof(DataMemberAttribute).GetProperty("Order"),
                                                         typeof(DataMemberAttribute).GetProperty("Name") };

        private readonly ConstructorInfo structLayoutCtor = typeof(StructLayoutAttribute)
                                                    .GetConstructor(new Type[] { typeof(LayoutKind) });
        private readonly FieldInfo[] structLayoutFields = new[] { typeof(StructLayoutAttribute).GetField("CharSet"),
                                                         typeof(StructLayoutAttribute).GetField("Pack") };

        private readonly ConstructorInfo marshalAsCtor = typeof(MarshalAsAttribute)
                                                            .GetConstructor(new Type[] { typeof(UnmanagedType) });

        private FieldBuilder[] fields = null;
        private PropertyBuilder[] props = null;       
        private FigureMode mode;
        private int length;
        private int scode = 1;
        private MemberRubrics members => figure.Rubrics;
        private InstantFigure figure;

        public InstantFigureValueTypeCompiler(InstantFigure instantFigure)
        {
            figure = instantFigure;
            length = members.Count;        
        }
      
        public Type CompileFigureType(string typeName)
        {
            fields = new FieldBuilder[length + scode];
            props = new PropertyBuilder[length + scode];

            TypeBuilder tb = GetTypeBuilder(typeName);

            CreateSerialCodeProperty(tb, typeof(Ussn), "SystemSerialCode");

            CreateFieldsAndProperties(tb, members);

            CreateValueArrayProperty(tb);

            CreateItemByIntProperty(tb);

            CreateItemByStringProperty(tb);

            CreateKeyBlockProperty(tb);

            CreateGetHashKeyMethod(tb);

            CreateSetHashKeyMethod(tb);

            CreateGetKeyBytesMethod(tb);

            CreateGetBytesMethod(tb);

            CreateGetEmptyProperty(tb);

            CreateEqualsMethod(tb);

            CreateCompareToMethod(tb);

            return tb.CreateTypeInfo();
        }

        private TypeBuilder GetTypeBuilder(string typeName)
        {
            string typeSignature = (typeName != null && typeName != "") ? typeName : "FigureFigure_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            AssemblyName an = new AssemblyName(typeSignature);

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder moduleBuilder     = assemblyBuilder.DefineDynamicModule(typeSignature + "Module");

            TypeBuilder tb;

            tb = moduleBuilder.DefineType(typeSignature, TypeAttributes.Public    | TypeAttributes.Serializable | TypeAttributes.BeforeFieldInit | TypeAttributes.Class |
                                                         TypeAttributes.AnsiClass | TypeAttributes.SequentialLayout, typeof(ValueType));

            //tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, new Type[] { })

            tb.SetCustomAttribute(new CustomAttributeBuilder(structLayoutCtor, new object[] { LayoutKind.Sequential }, 
                                                             structLayoutFields, new object[] { CharSet.Ansi, 1 }));
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(DataContractAttribute)
                                                                .GetConstructor(Type.EmptyTypes), new object[0]));

            tb.AddInterfaceImplementation(typeof(IFigure));

            return tb;
        }

        private void CreateSerialCodeProperty(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, null, type,  name);
            fields[0] = fb;

            PropertyBuilder prop = tb.DefineProperty(name,  PropertyAttributes.HasDefault,
                                                     type, new Type[] { type });

            PropertyInfo iprop = typeof(IFigure).GetProperty("SystemSerialCode");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            //tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldfld, fb); // load
            il.Emit(OpCodes.Ret); // return

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                               mutator.CallingConvention, mutator.ReturnType, argTypes);
            //tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Stfld, fb); // assign
            il.Emit(OpCodes.Ret);

            //prop.SetCustomAttribute(new CustomAttributeBuilder(
            //                           dataMemberCtor, new object[0],
            //                           dataMemberProps, new object[2] { 0, "SERIALCODE" }));

            props[0] = prop;
        }

        private FieldBuilder[] CreateFieldsAndProperties(TypeBuilder tb, MemberRubrics members)
        {
            for (int i = scode; i < length + scode; i++)
            {
                MemberInfo mi = members[i - scode];
                Type type = null;
                string name = mi.Name;
                if (mi.MemberType == MemberTypes.Field)
                {
                    if (mi is MemberRubric)
                        type = ((MemberRubric)mi).RubricType;
                    else
                        type = ((FieldInfo)mi).FieldType;
                }
                else if (mi.MemberType == MemberTypes.Property)
                {
                    if (mi is MemberRubric)
                        type = ((MemberRubric)mi).RubricType;
                    else
                        type = ((PropertyInfo)mi).PropertyType;
                }
                if (type.IsArray && !type.GetElementType().IsValueType)
                    type = null;
                else if (!type.IsArray && !type.IsValueType && type != typeof(string))
                    type = null;

                if (type != null)
                {
                    FieldBuilder fb = CreateField(tb, mi, type, name);
                    if (fb != null)
                    {

                        PropertyBuilder pi = (type != typeof(string)) ? CreateProperty(tb, fb, type, name) : CreateStringProperty(tb, fb, type, name);
                        fields[i] = fb;                                           
                        props[i] = pi;
                        pi.SetCustomAttribute(new CustomAttributeBuilder(dataMemberCtor, new object[0], dataMemberProps, new object[2] { i - scode, name }));
                    }
                }
            }

            return fields;
        }
       
        private FieldBuilder CreateField(TypeBuilder tb, MemberInfo member, Type type, string name)
        {
            if (type == typeof(string))
            {
                return CreateStringField(tb, member, type, name);
            }
            else if(type.IsArray)
            { 
                FieldBuilder field = null;

                object[] o = member.GetCustomAttributes(false).Where(r => r is MarshalAsAttribute).ToArray();
                if (!o.Any())
                {
                    var t  = member.GetCustomAttributes(false).Where(r => r is StructuringAttribute);
                    StructuringAttribute[] maa = t.Any() ? t.Cast<StructuringAttribute>().Where(r => r.Value == UnmanagedType.ByValTStr ||
                                                                                                     r.Value == UnmanagedType.ByValArray).ToArray() : null;
                    if (maa != null)
                    {
                        field = tb.DefineField("_" + name, type, FieldAttributes.Public | FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal);
                        CreateStructuringAttribute(field, member, maa.First());
                    }                    
                }
                else
                {
                    MarshalAsAttribute[] maa = o.Cast<MarshalAsAttribute>()
                                                    .Where(r => r.Value == UnmanagedType.ByValTStr ||
                                                                r.Value == UnmanagedType.ByValArray).ToArray();
                    if (maa.Any())
                    {
                        field = tb.DefineField("_" + name, type, FieldAttributes.Public | FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal);
                        CreateMarshalAttribute(field, member, maa.First());                        
                    }                   
                }
                return field;
            }            

            return tb.DefineField("_" + name, type, FieldAttributes.Public | FieldAttributes.HasDefault);
        }

        private FieldBuilder CreateStringField(TypeBuilder tb, MemberInfo member, Type type, string name)
        {

            FieldBuilder field = null;

            object[] o = member.GetCustomAttributes(false).Where(r => r is MarshalAsAttribute).ToArray();
            if (!o.Any())
            {
                var t = member.GetCustomAttributes(false).Where(r => r is StructuringAttribute);
                StructuringAttribute[] maa = t.Any() ? t.Cast<StructuringAttribute>().Where(r => r.Value == UnmanagedType.ByValTStr ||
                                                                                                 r.Value == UnmanagedType.ByValArray).ToArray() : null;
                if (maa != null)
                {
                    field = tb.DefineField("_" + name, typeof(char[]), FieldAttributes.Public | FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal);
                    CreateStructuringAttribute(field, member, new StructuringAttribute(UnmanagedType.ByValArray) { SizeConst = maa.First().SizeConst });
                }
            }
            else
            {
                MarshalAsAttribute[] maa = o.Cast<MarshalAsAttribute>().Where(r => r.Value == UnmanagedType.ByValTStr ||
                                                                                   r.Value == UnmanagedType.ByValArray).ToArray();
                if (maa.Any())
                {

                    field = tb.DefineField("_" + name, typeof(char[]), FieldAttributes.Public | FieldAttributes.HasDefault | FieldAttributes.HasFieldMarshal);
                    CreateMarshalAttribute(field, member, new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = maa.First().SizeConst });
                }
            }
            return field;
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

        private PropertyBuilder CreateStringProperty(TypeBuilder tb, FieldBuilder field, Type type, string name)
        {                       
            PropertyBuilder prop = tb.DefineProperty(name, PropertyAttributes.HasDefault,
                                                     type, new Type[] { type });

            MethodBuilder getter = tb.DefineMethod("get_" + name, MethodAttributes.Public |
                                                            MethodAttributes.HideBySig, type,
                                                            Type.EmptyTypes);
            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            if (type == typeof(string))
            {
                il.Emit(OpCodes.Ldfld, field); // load
                il.Emit(OpCodes.Newobj, typeof(string).GetConstructor(new Type[] { typeof(char[]) })); // load
            }
            else
                il.Emit(OpCodes.Ldfld, field); // load
            il.Emit(OpCodes.Ret); // return

            MethodBuilder setter = tb.DefineMethod("set_" + name, MethodAttributes.Public |
                                                            MethodAttributes.HideBySig, typeof(void),
                                                            new Type[] { type });
            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            if (type == typeof(string))
            {
                il.Emit(OpCodes.Ldarg_1); // value
                il.EmitCall(OpCodes.Call, typeof(string).GetMethod("ToCharArray", Type.EmptyTypes), null);
            }           
            il.Emit(OpCodes.Stfld, field); // assign
            il.Emit(OpCodes.Ret);

            return prop;

        }

        private void CreateMarshalAttribute(FieldBuilder field, MemberInfo member, MarshalAsAttribute attrib)
        {
            List<object> attribValues = new List<object>(2);
            List<FieldInfo> attribFields = new List<FieldInfo>(2);
            attribValues.Add(attrib.SizeConst);
            attribFields.Add(attrib.GetType().GetField("SizeConst"));
            //if (attrib.Value == UnmanagedType.ByValArray)
            //{
            //    attribValues.Add(attrib.ArraySubType);
            //    attribFields.Add(attrib.GetType().GetField("ArraySubType"));
            //}
            field.SetCustomAttribute(new CustomAttributeBuilder(marshalAsCtor, new object[] { attrib.Value }, attribFields.ToArray(), attribValues.ToArray()));
        }

        private void CreateStructuringAttribute(FieldBuilder field, MemberInfo member, StructuringAttribute attrib)
        {
            List<object> attribValues = new List<object>(2);
            List<FieldInfo> attribFields = new List<FieldInfo>(2);
            attribValues.Add(attrib.SizeConst);
            attribFields.Add(typeof(MarshalAsAttribute).GetField("SizeConst"));
            //if (attrib.Value == UnmanagedType.ByValArray)
            //{
            //    attribValues.Add(attrib.ArraySubType);
            //    attribFields.Add(typeof(MarshalAsAttribute).GetField("ArraySubType"));
            //}
            field.SetCustomAttribute(new CustomAttributeBuilder(marshalAsCtor, new object[] { attrib.Value }, attribFields.ToArray(), attribValues.ToArray()));
        }

        private void CreateValueArrayProperty(TypeBuilder tb)
        {
            PropertyInfo prop = typeof(IFigure).GetProperty("ValueArray");

            MethodInfo accessor = prop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            tb.DefineMethodOverride(method, accessor);

            ILGenerator il = method.GetILGenerator();
            il.DeclareLocal(typeof(object[]));

            il.Emit(OpCodes.Ldc_I4, length);
            il.Emit(OpCodes.Newarr, typeof(object));
            il.Emit(OpCodes.Stloc_0);

            for (int i = scode; i < length + scode; i++)
            {
                il.Emit(OpCodes.Ldloc_0); // this
                il.Emit(OpCodes.Ldc_I4, i - scode);
                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldfld, fields[i]); // foo load
                if (fields[i].FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Box, fields[i].FieldType); // box
                }
                else if (fields[i].FieldType == typeof(char[]))
                {
                    il.Emit(OpCodes.Newobj, typeof(string).GetConstructor(new Type[] { typeof(char[]) })); // load
                }   
                il.Emit(OpCodes.Stelem, typeof(object)); // this
            }
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret); // return

            MethodInfo mutator = prop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                               mutator.CallingConvention, mutator.ReturnType, argTypes);
            tb.DefineMethodOverride(method, mutator);
            il = method.GetILGenerator();
            il.DeclareLocal(typeof(object[]));
           
            il.Emit(OpCodes.Ldarg_1); // value
            il.Emit(OpCodes.Stloc_0);
            for (int i = scode; i < length + scode; i++)
            {
                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Ldc_I4, i - scode);
                il.Emit(OpCodes.Ldelem, typeof(object));
                if (fields[i].FieldType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, fields[i].FieldType);
                }
                else if (fields[i].FieldType == typeof(char[]))
                {
                    il.Emit(OpCodes.Castclass, typeof(string));
                    il.EmitCall(OpCodes.Call, typeof(string).GetMethod("ToCharArray", Type.EmptyTypes), null);
                }
                else
                    il.Emit(OpCodes.Castclass, fields[i].FieldType);
                il.Emit(OpCodes.Stfld, fields[i]); // 
            }
            il.Emit(OpCodes.Ret);
        }

        private void CreateItemByIntProperty(TypeBuilder tb)
        {
            foreach (PropertyInfo prop in typeof(IFigure).GetProperties())
            {
                MethodInfo accessor = prop.GetGetMethod();
                if (accessor != null)
                {
                    ParameterInfo[] args = accessor.GetParameters();
                    Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                    if (args.Length == 1 && argTypes[0] == typeof(int))
                    {
                        MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
                        tb.DefineMethodOverride(method, accessor);
                        ILGenerator il = method.GetILGenerator();

                        Label[] branches = new Label[length];
                        for (int i = scode; i < length + scode; i++)
                        {
                            if (fields[i].FieldType != null)
                                branches[i - scode] = il.DefineLabel();
                        }
                        il.Emit(OpCodes.Ldarg_1); // key

                        il.Emit(OpCodes.Switch, branches); // switch
                                                           // default:
                        il.ThrowException(typeof(ArgumentOutOfRangeException));
                        for (int i = scode; i < length + scode; i++)
                        {
                            if (fields[i].FieldType != null)
                            {
                                il.MarkLabel(branches[i - scode]);
                                il.Emit(OpCodes.Ldarg_0); // this
                                il.Emit(OpCodes.Ldfld, fields[i]); // foo load
                                if (fields[i].FieldType.IsValueType)
                                {
                                    il.Emit(OpCodes.Box, fields[i].FieldType); // box
                                }
                                else if (fields[i].FieldType == typeof(char[]))
                                {
                                    il.Emit(OpCodes.Newobj, typeof(string).GetConstructor(new Type[] { typeof(char[]) })); // load
                                }
                                il.Emit(OpCodes.Ret); // end
                            }
                        }
                    }                   
                }


                MethodInfo mutator = prop.GetSetMethod();
                if (mutator != null)
                {
                    ParameterInfo[] args = mutator.GetParameters();
                    Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                    if (args.Length == 2 && argTypes[0] == typeof(int) && argTypes[1] == typeof(object))
                    {
                        MethodBuilder method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                           mutator.CallingConvention, mutator.ReturnType, argTypes);
                        tb.DefineMethodOverride(method, mutator);
                        ILGenerator il = method.GetILGenerator();                       

                        Label[] branches = new Label[length];
                        for (int i = scode; i < length + scode; i++)
                        {
                            if (fields[i].FieldType != null)
                                branches[i - scode] = il.DefineLabel();
                        }
                        il.Emit(OpCodes.Ldarg_1); // key

                        il.Emit(OpCodes.Switch, branches); // switch
                                                           // default:
                        il.ThrowException(typeof(ArgumentOutOfRangeException));
                        for (int i = scode; i < length + scode; i++)
                        {
                            if (fields[i].FieldType != null)
                            {
                                il.MarkLabel(branches[i - scode]);
                                il.Emit(OpCodes.Ldarg_0); // this
                                il.Emit(OpCodes.Ldarg_2); // value
                                if (fields[i].FieldType.IsValueType)
                                {
                                    il.Emit(OpCodes.Unbox_Any, fields[i].FieldType); 
                                }
                                else if (fields[i].FieldType == typeof(char[]))
                                {
                                    il.Emit(OpCodes.Castclass, typeof(string)); 
                                    il.EmitCall(OpCodes.Call, typeof(string).GetMethod("ToCharArray", Type.EmptyTypes), null);
                                }
                                else
                                    il.Emit(OpCodes.Castclass, fields[i].FieldType);                                 
                                il.Emit(OpCodes.Stfld, fields[i]); 
                                il.Emit(OpCodes.Ret); // end
                            }
                        }
                    }                    
                }
                
            }                          
        }

        private void CreateItemByStringProperty(TypeBuilder tb)
        {
            foreach (PropertyInfo prop in typeof(IFigure).GetProperties())
            {
                MethodInfo accessor = prop.GetGetMethod();
                if (accessor != null)
                {
                    ParameterInfo[] args = accessor.GetParameters();
                    Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);
                    
                    if (args.Length == 1 && argTypes[0] == typeof(string))
                    {
                        MethodBuilder method = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                           accessor.CallingConvention, accessor.ReturnType, argTypes);
                        tb.DefineMethodOverride(method, accessor);
                        ILGenerator il = method.GetILGenerator();

                        il.DeclareLocal(typeof(string));

                        Label[] branches = new Label[length + scode];

                        for (int i = 0; i < length + scode; i++)
                        {
                            branches[i] = il.DefineLabel();
                        }

                        il.Emit(OpCodes.Ldarg_1); // key
                        il.Emit(OpCodes.Stloc_0);

                        for (int i = 0; i < length + scode; i++)
                        {
                            if (props[i].Name != null)
                            {
                                il.Emit(OpCodes.Ldloc_0);
                                il.Emit(OpCodes.Ldstr, props[i].Name);
                                il.EmitCall(OpCodes.Call, typeof(string).GetMethod("op_Equality", new Type[] { typeof(string), typeof(string) }), null);
                                il.Emit(OpCodes.Brtrue, branches[i]);
                            }
                        }

                        il.Emit(OpCodes.Ldnull); // this
                        il.Emit(OpCodes.Ret);

                        for (int i = 0; i < length + scode; i++)
                        {
                            if (props[i].Name != null)
                            {
                                il.MarkLabel(branches[i]);
                                il.Emit(OpCodes.Ldarg_0); // this
                                il.Emit(OpCodes.Ldfld, fields[i]); // foo load
                                if (fields[i].FieldType.IsValueType)
                                {
                                    il.Emit(OpCodes.Box, fields[i].FieldType); // box
                                }
                                else if (fields[i].FieldType == typeof(char[]))
                                {
                                    il.Emit(OpCodes.Newobj, typeof(string).GetConstructor(new Type[] { typeof(char[]) })); // load
                                }
                                il.Emit(OpCodes.Ret); // end
                            }
                        }
                    }
                }


                MethodInfo mutator = prop.GetSetMethod();
                if (mutator != null)
                {
                    ParameterInfo[] args = mutator.GetParameters();
                    Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                    if (args.Length == 2 && argTypes[0] == typeof(string) && argTypes[1] == typeof(object))
                    {
                        MethodBuilder method = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                           mutator.CallingConvention, mutator.ReturnType, argTypes);
                        tb.DefineMethodOverride(method, mutator);
                        ILGenerator il = method.GetILGenerator();

                        il.DeclareLocal(typeof(string));

                        Label[] branches = new Label[length + scode];
                        for (int i = 0; i < length + scode; i++)
                        {
                            if (props[i].Name != null)
                                branches[i] = il.DefineLabel();
                        }

                        il.Emit(OpCodes.Ldarg_1); // key
                        il.Emit(OpCodes.Stloc_0);

                        for (int i = 0; i < length + scode; i++)
                        {
                            if (props[i].Name != null)
                            {
                                il.Emit(OpCodes.Ldloc_0);
                                il.Emit(OpCodes.Ldstr, props[i].Name);
                                il.EmitCall(OpCodes.Call, typeof(string).GetMethod("op_Equality", new[] { typeof(string), typeof(string) }), null);
                                il.Emit(OpCodes.Brtrue, branches[i]);
                            }
                        }

                        il.Emit(OpCodes.Ret);

                        for (int i = 0; i < length + scode; i++)
                        {
                            if (props[i].Name != null)
                            {
                                il.MarkLabel(branches[i]);
                                il.Emit(OpCodes.Ldarg_0); // this
                                il.Emit(OpCodes.Ldarg_2); // value
                                if (fields[i].FieldType.IsValueType)
                                {
                                    il.Emit(OpCodes.Unbox_Any, fields[i].FieldType); 
                                }
                                else if (fields[i].FieldType == typeof(char[]))
                                {
                                    il.Emit(OpCodes.Castclass, typeof(string));     
                                    il.EmitCall(OpCodes.Call, typeof(string).GetMethod("ToCharArray", Type.EmptyTypes), null);
                                }
                                else
                                    il.Emit(OpCodes.Castclass, fields[i].FieldType);                      
                                il.Emit(OpCodes.Stfld, fields[i]); // 
                                il.Emit(OpCodes.Ret); // end
                            }
                        }
                    }
                }

            }
        }

        private void CreateGetBytesMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IUnique).GetMethod("GetBytes");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("GetBytes", Type.EmptyTypes), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateGetKeyBytesMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IUnique).GetMethod("GetKeyBytes");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("GetKeyBytes"), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateGetHashKeyMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IUnique).GetMethod("GetHashKey");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("GetHashKey"), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateSetHashKeyMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IUnique).GetMethod("SetHashKey");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("SetHashKey"), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateEqualsMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IEquatable<IUnique>).GetMethod("Equals");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("Equals", new Type[] { typeof(IUnique) }), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateCompareToMethod(TypeBuilder tb)
        {
            MethodInfo createArray = typeof(IComparable<IUnique>).GetMethod("CompareTo");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(createArray.Name, createArray.Attributes & ~MethodAttributes.Abstract,
                                                          createArray.CallingConvention, createArray.ReturnType, argTypes);
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, fields[0]);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("CompareTo", new Type[] { typeof(IUnique) }), null);
            il.Emit(OpCodes.Ret);
        }

        private void CreateGetEmptyProperty(TypeBuilder tb)
        {
            PropertyBuilder prop = tb.DefineProperty("Empty", PropertyAttributes.HasDefault,
                                                     typeof(IUnique), Type.EmptyTypes);

            PropertyInfo iprop = typeof(IUnique).GetProperty("Empty");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldflda, fields[0]); // load
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetMethod("get_Empty"), null);
            il.Emit(OpCodes.Ret); // return
        }

        private void CreateKeyBlockProperty(TypeBuilder tb)
        {

            PropertyBuilder prop = tb.DefineProperty("KeyBlock", PropertyAttributes.HasDefault,
                                                     typeof(long), new Type[] { typeof(long) });

            PropertyInfo iprop = typeof(IUnique).GetProperty("KeyBlock");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(accessor.Name, accessor.Attributes & ~MethodAttributes.Abstract,
                                                          accessor.CallingConvention, accessor.ReturnType, argTypes);
            //tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();           

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldflda, fields[0]); // load
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetProperty("KeyBlock").GetGetMethod(), null);
            il.Emit(OpCodes.Ret); // return

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(mutator.Name, mutator.Attributes & ~MethodAttributes.Abstract,
                                                          mutator.CallingConvention, mutator.ReturnType, argTypes);
            //tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0); // this
            il.Emit(OpCodes.Ldflda, fields[0]); // load
            il.Emit(OpCodes.Ldarg_1); // value
            il.EmitCall(OpCodes.Call, typeof(Ussn).GetProperty("KeyBlock").GetSetMethod(), null);
            il.Emit(OpCodes.Ret); // return

           // return prop;
        }

    }

}