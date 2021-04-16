using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Uniques;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Instants
{
    public class PropertyRubric : PropertyInfo, IMemberRubric
    {
        public PropertyRubric() { }
        public PropertyRubric(PropertyInfo property, int size = -1, int propertyId = -1) : this(property.PropertyType, property.Name, propertyId)
        {
            RubricInfo = property;
        }
        public PropertyRubric(Type propertyType, string propertyName, int size = -1, int propertyId = -1)
        {
            RubricType = propertyType;
            RubricName = propertyName;
            RubricId = propertyId;
            if (propertyType.IsValueType)
            {
                if (propertyType == typeof(DateTime))
                    RubricSize = 8;
                else
                    RubricSize = Marshal.SizeOf(propertyType);
            }
            if (size > 0)
                RubricSize = size;
        }

        public string RubricName { get; set; }
        public Type RubricType { get; set; }
        public PropertyInfo RubricInfo { get; set; }
        public int RubricId { get; set; } = -1;
        public int RubricSize { get; set; } = -1;
        public int RubricOffset { get; set; } = -1;
        public bool Visible { get; set; } = true;
        public bool Editable { get; set; } = true;
        public object[] RubricAttributes { get; set; } = null;

        public override Type PropertyType => RubricType;

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            if (RubricId < 0)
                return ((IFigure)obj)[RubricName];
            return ((IFigure)obj)[RubricId];
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
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
                        if (attrib.Where(r => r is StructuringAttribute).Any())
                        {
                            attrib.Cast<StructuringAttribute>().Select(a => RubricSize = a.SizeConst).ToArray();
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
                return new[] { new MarshalAsAttribute(UnmanagedType.ByValTStr) { SizeConst = RubricSize } };
            }
            else if (RubricType.IsArray)
            {
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

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            if (RubricInfo != null)
                return RubricInfo.GetAccessors(nonPublic);
            return null;
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            if (RubricInfo != null)
                return RubricInfo.GetGetMethod(nonPublic);
            return null;
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            if (RubricInfo != null)
                return RubricInfo.GetIndexParameters();
            return null;
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            if (RubricInfo != null)
                return RubricInfo.GetSetMethod(nonPublic);
            return null;
        }

        public override PropertyAttributes Attributes => RubricInfo != null ? RubricInfo.Attributes : PropertyAttributes.HasDefault;     

        public override Type DeclaringType => RubricInfo != null ? RubricInfo.DeclaringType : null;

        public override string Name => RubricName;

        public override Type ReflectedType => RubricInfo != null ? RubricInfo.ReflectedType : null;

        public override bool CanRead => Visible;

        public override bool CanWrite => Editable;

        public Type RubricReturnType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public PropertyInfo[] RubricParameterInfo { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Module RubricModule { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }        
    }
     
}
