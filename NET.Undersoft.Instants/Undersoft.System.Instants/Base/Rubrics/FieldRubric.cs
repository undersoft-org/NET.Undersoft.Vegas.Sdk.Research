using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Instants
{
    public class FieldRubric : FieldInfo, IMemberRubric
    {
        public FieldRubric() { }
        public FieldRubric(Type fieldType, string fieldName, int size = -1, int fieldId = -1)
        {
            RubricType = fieldType;
            RubricName = fieldName;
            RubricId = fieldId;
            if (fieldType.IsValueType)
            {
                if (fieldType == typeof(DateTime))
                    RubricSize = 8;
                else
                    RubricSize = Marshal.SizeOf(fieldType);
            }
            if (size > 0)
                RubricSize = size;
        }
        public FieldRubric(FieldInfo field, int size = -1, int fieldId = -1) : this(field.FieldType, field.Name, size, fieldId)
        {
            RubricInfo = field;
        }

        public string RubricName { get; set; }
        public Type RubricType { get; set; }
        public FieldInfo RubricInfo { get; set; }
        public int RubricId { get; set; }
        public int RubricSize { get; set; }
        public int RubricOffset { get; set; }
        public bool Visible { get; set; } = true;
        public bool Editable { get; set; } = true;
        public object[] RubricAttributes { get; set; }

        public override Type FieldType => RubricType;

        public override object GetValue(object obj)
        {
            if (RubricId < 0)
                return ((IFigure)obj)[RubricName];
            return ((IFigure)obj)[RubricId];
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            if (RubricId < 0)
                ((IFigure)obj)[RubricName] = value;
            ((IFigure)obj)[RubricId] = value;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            if (RubricAttributes != null)
                return RubricAttributes;

            RubricAttributes = new object[0];
            if (RubricInfo != null)
            {
                var attrib = RubricInfo.GetCustomAttributes(inherit);
                if (attrib != null)
                {
                    if (RubricType.IsArray || RubricType == typeof(string))
                    {
                        if (attrib.Where(r => r is MarshalAsAttribute).Any())
                        {
                            attrib.Cast<MarshalAsAttribute>().Select(a => RubricSize = a.SizeConst).ToArray();
                            return RubricAttributes = attrib;
                        }
                        else
                            RubricAttributes.Concat(attrib).ToArray();
                    }
                    else
                        return RubricAttributes.Concat(attrib).ToArray();
                }
            }

            if (RubricType == typeof(string))
            {
                if (RubricSize < 1)
                    RubricSize = 16;
                return new[] { new MarshalAsAttribute(UnmanagedType.ByValTStr) { SizeConst = RubricSize } };
            }
            else if (RubricType.IsArray)
            {
                if (RubricSize < 1)
                    RubricSize = 8;

                if (RubricType == typeof(byte[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize, ArraySubType = UnmanagedType.U1 } }).ToArray();
                if (RubricType == typeof(char[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize, ArraySubType = UnmanagedType.U1 } }).ToArray();
                if (RubricType == typeof(int[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize / 4, ArraySubType = UnmanagedType.I4 } }).ToArray();
                if (RubricType == typeof(long[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize / 8, ArraySubType = UnmanagedType.I8 } }).ToArray();
                if (RubricType == typeof(float[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize / 4, ArraySubType = UnmanagedType.R4 } }).ToArray();
                if (RubricType == typeof(double[]))
                    return RubricAttributes.Concat(new[] { new MarshalAsAttribute(UnmanagedType.ByValArray) { SizeConst = RubricSize / 8, ArraySubType = UnmanagedType.R8 } }).ToArray();
            }
            return null;
        }
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            var attribs = this.GetCustomAttributes(inherit);
            if (attribs != null)
                attribs = attribs.Where(r => r.GetType() == attributeType).ToArray();
            if (!attribs.Any())
                return null;
            return attribs;
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (this.GetCustomAttributes(attributeType, inherit) != null)
                return true;
            return false;
        }

        public override FieldAttributes Attributes => RubricInfo != null ? RubricInfo.Attributes : FieldAttributes.HasDefault;

        public override RuntimeFieldHandle FieldHandle => RubricInfo != null ? RubricInfo.FieldHandle : throw new NotImplementedException();

        public override Type DeclaringType => RubricInfo != null ? RubricInfo.DeclaringType : null;

        public override string Name => RubricName;

        public override Type ReflectedType => RubricInfo != null ? RubricInfo.ReflectedType : null;

        public Type RubricReturnType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PropertyInfo[] RubricParameterInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Module RubricModule { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    }
}
