using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using Xunit;
using System.Globalization;
using System.Instants;

namespace System.Extract
{

    public static class InstantFigureMocks
    {   
        public static MemberInfo[] InstantFigure_MemberRubric_FieldsAndPropertiesModel()
        {
            return typeof(FieldsAndPropertiesModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? new MemberRubric((FieldInfo)m) :
                                                             m.MemberType == MemberTypes.Property ? new MemberRubric((PropertyInfo)m) :
                                                             null).Where(p => p != null).ToArray();
        }

    }
}
