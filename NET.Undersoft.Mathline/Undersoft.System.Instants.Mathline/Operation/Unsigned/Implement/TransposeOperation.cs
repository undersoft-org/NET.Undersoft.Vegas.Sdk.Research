using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;


namespace System.Instants.Mathline
{
    [Serializable]
    public class TransposeOperation : UnsignedOperator
    {
        public TransposeOperation(Formula e) : base(e)
        {

        }
        
        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                e.Compile(g, cc);
                return;
            }

            // swap the indexes at the compiler level
            int i1 = cc.GetIndexVariable(0);
            int i2 = cc.GetIndexVariable(1);
            cc.SetIndexVariable(1, i1);
            cc.SetIndexVariable(0, i2);
            e.Compile(g, cc);
            cc.SetIndexVariable(0, i1);
            cc.SetIndexVariable(1, i2);
        }

        //public override double Math(int i, int j)
        //{
        //    return e.Math(j, i);
        //}

        public override MathlineSize Size
        {
            get
            {
                MathlineSize o = e.Size;
                return new MathlineSize(o.cols, o.rows);
            }
        }

    }
}
