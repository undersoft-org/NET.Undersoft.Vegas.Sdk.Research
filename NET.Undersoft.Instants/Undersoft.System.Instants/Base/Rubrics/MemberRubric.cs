using System.Text;
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
    public class MemberRubric : MemberInfo, IMemberRubric, IUnique
    {                       
        public MemberRubric(IMemberRubric member)
        {
            RubricInfo = ((MemberInfo)member);
            RubricName = member.RubricName;
            RubricId = member.RubricId;
            Visible = member.Visible;
            Editable = member.Editable;
            if (RubricInfo.MemberType == MemberTypes.Method)
                SystemSerialCode = new Ussn((new String(RubricParameterInfo
                                            .SelectMany(p => p.ParameterType.Name)
                                                .ToArray()) + "_" + RubricName).GetHashKey64());
            else
                SystemSerialCode = new Ussn(RubricName.GetHashKey64());
        }
        public MemberRubric(MemberRubric member) : this((IMemberRubric)member)
        {
            FigureType = member.FigureType;
            FigureField = member.FigureField;
            FigureFieldId = member.FigureFieldId;
            RubricOffset = member.RubricOffset;
        }
        public MemberRubric(MethodRubric method) : this((IMemberRubric)method)
        {
        }
        public MemberRubric(FieldRubric field) : this((IMemberRubric)field)
        {
        }
        public MemberRubric(PropertyRubric property) : this((IMemberRubric)property)
        {
        }
        public MemberRubric(MethodInfo method) : this((IMemberRubric)new MethodRubric(method))
        {
        }
        public MemberRubric(PropertyInfo property) : this((IMemberRubric)new PropertyRubric(property))
        {
        }
        public MemberRubric(FieldInfo field) : this((IMemberRubric)new FieldRubric(field))
        {
        }

        public MemberRubrics Rubrics { get; set; }

        public Type FigureType { get; set; }
        public FieldInfo FigureField { get; set; }
        public int FigureFieldId { get; set; }
        public MemberInfo RubricInfo { get; set; }
        public IMemberRubric VirtualInfo => (IMemberRubric)RubricInfo;

        public Type RubricReturnType { get => MemberType == MemberTypes.Method ? ((MethodRubric)RubricInfo).RubricReturnType : null; }
        public ParameterInfo[] RubricParameterInfo { get => MemberType == MemberTypes.Method ? ((MethodRubric)RubricInfo).RubricParameterInfo : null; }
        public Module RubricModule { get => MemberType == MemberTypes.Method ? ((MethodRubric)RubricInfo).RubricModule : null; }
        public string RubricName { get; set; }
        public Type RubricType { get { return VirtualInfo.RubricType; } set { VirtualInfo.RubricType = value; } }

        public int RubricId { get; set; }
        public int RubricSize { get { return VirtualInfo.RubricSize; } set { VirtualInfo.RubricSize = value; } }
        public int RubricOffset { get; set; }
        public bool Visible { get; set; }
        public bool Editable { get; set; }
        public bool IsKey { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsAutoincrement { get; set; }
        public bool IsDBNull { get; set; }
        public bool IsMultiDeck { get; set; }
        public object[] RubricAttributes { get { return VirtualInfo.RubricAttributes; } set { VirtualInfo.RubricAttributes = value; } }

        public override Type DeclaringType => FigureType != null ? FigureType : RubricInfo.DeclaringType;
        public override MemberTypes MemberType => RubricInfo.MemberType;
        public override string Name => RubricInfo.Name;
        public override Type ReflectedType => RubricInfo.ReflectedType;

        public IUnique Empty => Ussn.Empty;

        public long KeyBlock { get => SystemSerialCode.KeyBlock; set => SystemSerialCode.KeyBlock = value; }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return RubricInfo.GetCustomAttributes(inherit);
        }
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return RubricInfo.GetCustomAttributes(attributeType, inherit);
        }
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return RubricInfo.IsDefined(attributeType, inherit);
        }

        public byte[] GetBytes()
        {
            return SystemSerialCode.GetBytes();
        }

        public byte[] GetKeyBytes()
        {
            return SystemSerialCode.GetKeyBytes();
        }

        public void SetHashKey(long value)
        {
            SystemSerialCode.KeyBlock = value;
        }

        public long GetHashKey()
        {
            return SystemSerialCode.KeyBlock;
        }

        public bool Equals(IUnique other)
        {
           return KeyBlock == other.KeyBlock;
        }

        public int CompareTo(IUnique other)
        {
            return (int)(KeyBlock - other.KeyBlock);
        }

        public Ussn SystemSerialCode;
       
    }
}
