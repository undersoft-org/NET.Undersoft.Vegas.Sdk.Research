using System.Reflection.Emit;
using System.Instants;

namespace System.Instants.Mathline
{
    [Serializable]
    public class SubMathline : LeftFormula
    {
        public IMultemic Data { get { return Formuler.Data; } }

        public MathRubric Rubric
        { get; set; }
        public Mathline       Formuler
        { get; set; }
        public SubMathline    SubFormuler
        { get; set; }

        public string RubricName
        { get => Rubric.RubricName; }
        public Type RubricType
        { get => Rubric.RubricType; }
        public int FieldId
        { get => Rubric.FigureFieldId; }

        public int rowCount { get { return Data.Count; } }
        public int colCount { get { return Formuler.Rubrics.Count; } }

        public int startId = 0;     
              
        public SubMathline(MathRubric evalRubric, Mathline formuler)
        {
            if (evalRubric != null) Rubric = evalRubric;
          
            SetDimensions(formuler);
        }

        public void SetDimensions(Mathline formuler = null)
        {
            if (!ReferenceEquals(formuler, null))
                Formuler = formuler;
            Rubric.SubFormuler = this;

        }  

        public override void CompileAssign(ILGenerator g, CompilerContext cc, bool post, bool partial)
        {
            if (cc.IsFirstPass())
            {
                cc.Add(Data);
                return;
            }

            int i1 = cc.GetIndexVariable(0);

            if (!post)
            {
                if (!partial)
                {
                    CompilerContext.GenLocalLoad(g, cc.GetIndexOf(Data));

                    if (startId != 0)
                        g.Emit(OpCodes.Ldc_I4, startId);

                    g.Emit(OpCodes.Ldloc, i1);

                    if (startId != 0)
                        g.Emit(OpCodes.Add);

                    g.EmitCall(OpCodes.Callvirt, typeof(IMultemic).GetMethod("get_Item", new Type[] { typeof(int) }), null);
                    CompilerContext.GenLocalStore(g, cc.GetSubIndexOf(Data));
                    CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));
                }
                else
                {
                    CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));
                }
                g.Emit(OpCodes.Ldc_I4, FieldId);

            }
            else
            {
                if (partial)
                {
                    g.Emit(OpCodes.Dup);
                    CompilerContext.GenLocalStore(g, cc.GetBufforIndexOf(Data));           // index
                }

                g.Emit(OpCodes.Box, typeof(double));
                g.EmitCall(OpCodes.Callvirt, typeof(IFigure).GetMethod("set_Item", new Type[] { typeof(int), typeof(object) }), null);


                if (partial)
                    CompilerContext.GenLocalLoad(g, cc.GetBufforIndexOf(Data));           // index
            }            
        }

        // Compilation First Pass: add a reference to the array variable
        // Code Generation: access the element through the i index
        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                cc.Add(Data);
            }
            else
            {
                CompilerContext.GenLocalLoad(g, cc.GetSubIndexOf(Data));           // index

                g.Emit(OpCodes.Ldc_I4, FieldId);
                g.EmitCall(OpCodes.Callvirt, typeof(IFigure).GetMethod("get_Item", new Type[] { typeof(int) }), null);
                g.Emit(OpCodes.Unbox_Any, RubricType);
                g.Emit(OpCodes.Conv_R8);
            }
        }

        public override MathlineSize Size
        {
            get { return new MathlineSize(rowCount, colCount); }
        }

        //public override double Math(int i, int j)
        //{
        //    return this[i, j];
        //}
    }
}
