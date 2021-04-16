using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace System.Instants.Mathline
{
    [Serializable]
    public class PowerOperation : BinaryFormula
    {
        public PowerOperation(Formula e1, Formula e2) : base(e1, e2)
        {

        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                expr1.Compile(g, cc);
                expr2.Compile(g, cc);
                if (!(expr2.Size == MathlineSize.Scalar))
                    throw new SizeMismatchException("Pow Operator requires a scalar second operand");
                return;
            }
            expr1.Compile(g, cc);
            expr2.Compile(g, cc);
            g.EmitCall(OpCodes.Call, typeof(Math).GetMethod("Pow"), null);
        }      

        public override MathlineSize Size
        {
            get { return expr1.Size; }
        }
    }
}
