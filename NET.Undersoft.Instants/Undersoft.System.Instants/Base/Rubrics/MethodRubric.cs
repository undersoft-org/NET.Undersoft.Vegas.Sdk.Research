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
    public class MethodRubric : MethodInfo, IMemberRubric
    {       
        public MethodRubric() { }
        public MethodRubric(MethodInfo method, int propertyId = -1) : 
            this(method.DeclaringType, method.Name, method.ReturnType, method.GetParameters(), method.Module, propertyId)
        {
            RubricInfo = method;
            RubricType = method.DeclaringType;
            RubricName = method.Name;
            RubricParameterInfo = method.GetParameters();
            RubricReturnType = method.ReturnType;
            RubricModule = method.Module;
        }
        public MethodRubric(Type declaringType, string propertyName, Type returnType, ParameterInfo[] parameterTypes, Module module, int propertyId = -1)
        {
            RubricType = declaringType;
            RubricName = propertyName;
            RubricId = propertyId;
            RubricParameterInfo = parameterTypes;
            RubricReturnType = returnType;
            RubricModule = module;
        }

        public Type RubricReturnType { get; set; }
        public ParameterInfo[] RubricParameterInfo { get; set; }
        public Module RubricModule { get; set; }
        public string RubricName { get; set; }
        public Type RubricType { get; set; }
        public MethodInfo RubricInfo { get; set; }
        public int RubricId { get; set; } = -1;
        public int RubricSize { get; set; } = -1;
        public int RubricOffset { get; set; } = -1;
        public bool Visible { get; set; } = true;
        public bool Editable { get; set; } = true;
        public object[] RubricAttributes { get; set; } = null;        

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            if (this.GetCustomAttributes(attributeType, inherit) != null)
                return true;
            return false;
        }

        public override MethodInfo GetBaseDefinition()
        {
            return RubricInfo.GetBaseDefinition();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return RubricInfo.GetMethodImplementationFlags();
        }

        public override ParameterInfo[] GetParameters()
        {
            return RubricInfo.GetParameters();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
        {
           return RubricInfo.Invoke(obj, invokeAttr, binder, parameters, culture);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return RubricInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return RubricInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override Type DeclaringType => RubricInfo != null ? RubricInfo.DeclaringType : null;

        public override string Name => RubricName;

        public override Type ReflectedType => RubricInfo != null ? RubricInfo.ReflectedType : null;

        public override ICustomAttributeProvider ReturnTypeCustomAttributes => RubricInfo.ReturnTypeCustomAttributes;

        public override MethodAttributes Attributes => RubricInfo.Attributes;

        public override RuntimeMethodHandle MethodHandle => RubricInfo.MethodHandle;
    }
     
}
