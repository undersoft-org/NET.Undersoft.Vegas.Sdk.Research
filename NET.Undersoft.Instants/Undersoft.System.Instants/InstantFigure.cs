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
    public class InstantFigure
    {
        private Type compiledType;
        private MemberRubric[] members;

        public FigureMode Mode;
        public IFigure Figure;
        public Type FigureType;
        public int FigureSize;
        public MemberRubrics Rubrics
        { get; set; }
        public string TypeName;

        public object NewObject()
        {
            return FigureType.New();
        }
        public IFigure New()
        {
            return (IFigure)FigureType.New();
        }

        public InstantFigure(IList<MemberInfo> membersInfo, FigureMode modeType = FigureMode.Reference) :
            this(membersInfo.ToArray(), null, modeType) {}
        public InstantFigure(IList<MemberInfo> membersInfo, string typeName, FigureMode modeType = FigureMode.Reference)
        {        
            TypeName = typeName;
            Mode = modeType;

            members = CreateMemberRurics(membersInfo);

            Rubrics = new MemberRubrics();
            Rubrics.KeyRubrics = new MemberRubrics();
            foreach (MemberRubric mr in members)
                Rubrics.Add(mr);

            if (modeType == FigureMode.Reference)
            {
                InstantFigureReferenceCompiler rtbld = new InstantFigureReferenceCompiler(this);
                compiledType = rtbld.CompileFigureType(typeName);
            }
            else
            {
                InstantFigureValueTypeCompiler rtbld = new InstantFigureValueTypeCompiler(this);
                compiledType = rtbld.CompileFigureType(typeName);
            }

            Figure = (IFigure)compiledType.New();
            FigureType = Figure.GetType();
            FigureSize = Marshal.SizeOf(FigureType);

            if (!membersInfo.Where(m => m.Name == "SystemSerialCode").Any())
                members = new MemberRubric[] { new MemberRubric(FigureType.GetProperty("SystemSerialCode")) }.Concat(members).ToArray();

            members.Select((m, y) => m.RubricId = y).ToArray();

            members.Select(m => m.FigureField = FigureType.GetField("_" + m.RubricName, BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();

            members.Where(m => m.FigureField != null)
                .Select((f,y) => new object[]
                {
                    f.FigureFieldId = y - 1,
                    f.RubricOffset = (int)Marshal.OffsetOf(FigureType, "_" + f.RubricName)
                }).ToArray();

            Rubrics.Insert(0, members[0]);           
        }

        public InstantFigure(MemberRubrics memberRubrics, string typeName, FigureMode modeType = FigureMode.Reference)
        {
            TypeName = typeName;
            Mode = modeType;

            members = memberRubrics.AsValues().ToArray();

            Rubrics = memberRubrics;

            if (modeType == FigureMode.Reference)
            {
                InstantFigureReferenceCompiler rtbld = new InstantFigureReferenceCompiler(this);
                compiledType = rtbld.CompileFigureType(typeName);
            }
            else
            {
                InstantFigureValueTypeCompiler rtbld = new InstantFigureValueTypeCompiler(this);
                compiledType = rtbld.CompileFigureType(typeName);
            }

            Figure = (IFigure)compiledType.New();
            FigureType = Figure.GetType();
            FigureSize = Marshal.SizeOf(FigureType);

            if(!memberRubrics.ContainsKey("SystemSerialCode"))
                members = new MemberRubric[] { new MemberRubric(FigureType.GetProperty("SystemSerialCode")) }.Concat(members).ToArray();

        }

        private MemberRubric[] CreateMemberRurics(IList<MemberInfo> membersInfo)
        {
            return membersInfo.Select(m => !(m is MemberRubric) ? m.MemberType == MemberTypes.Field ? new MemberRubric((FieldInfo)m) :
                                                                  m.MemberType == MemberTypes.Property ? new MemberRubric((PropertyInfo)m) :
                                                                  null : (MemberRubric)m).Where(p => p != null).ToArray();

        }       

    }
}