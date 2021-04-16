using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Reflection;
using Xunit;
using System.Globalization;

namespace System.Instants
{

    public static class InstantFigureMocks
    {
        public static MemberInfo[] InstantFigure_Memberinfo_FieldsOnlyModel()
        {
            return typeof(FieldsOnlyModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                                            .Where(p => p != null).ToArray();
        }


        public static MemberInfo[] InstantFigure_MemberRubric_FieldsOnlyModel()
        {
            return typeof(FieldsOnlyModel).GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                                            .Select(m => new MemberRubric(m)).Where(p => p != null).ToArray();
        }

        public static MemberInfo[] InstantFigure_Memberinfo_PropertiesOnlyModel()
        {
            return typeof(PropertiesOnlyModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? m :
                                                             m.MemberType == MemberTypes.Property ? m :
                                                             null).Where(p => p != null).ToArray();
        }


        public static MemberInfo[] InstantFigure_MemberRubric_PropertiesOnlyModel()
        {
            return typeof(PropertiesOnlyModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? new MemberRubric((FieldInfo)m) :
                                                             m.MemberType == MemberTypes.Property ? new MemberRubric((PropertyInfo)m) :
                                                             null).Where(p => p != null).ToArray();
        }

        public static MemberInfo[] InstantFigure_Memberinfo_FieldsAndPropertiesModel()
        {
            return typeof(FieldsAndPropertiesModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? m :
                                                             m.MemberType == MemberTypes.Property ? m :
                                                             null).Where(p => p != null).ToArray();
        }


        public static MemberInfo[] InstantFigure_MemberRubric_FieldsAndPropertiesModel()
        {
            return typeof(FieldsAndPropertiesModel).GetMembers().Select(m => m.MemberType == MemberTypes.Field ? new MemberRubric((FieldInfo)m) :
                                                             m.MemberType == MemberTypes.Property ? new MemberRubric((PropertyInfo)m) :
                                                             null).Where(p => p != null).ToArray();
        }

    }
}
