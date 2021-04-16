using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace System.Instants.Mathline
{
    [Serializable]
    public class Equal : BinaryOperator
    {
        public override double Apply(double a, double b)
        {
            return Convert.ToDouble(a == b);
        }
        public override void Compile(ILGenerator g)
        {
            g.Emit(OpCodes.Ceq);
            g.Emit(OpCodes.Conv_R8);
        }
    }
}
