using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace System.Instants.Mathline
{
    [Serializable]
    public class AndOperand : BinaryOperator
    {
        public override double Apply(double a, double b)
        {
            return Convert.ToDouble(Convert.ToBoolean(a) && Convert.ToBoolean(b));
        }
        public override void Compile(ILGenerator g)
        {
            //g.Emit(OpCodes.Conv_I4);
            g.Emit(OpCodes.And);
            g.Emit(OpCodes.Conv_R8);
        }
    }
}
