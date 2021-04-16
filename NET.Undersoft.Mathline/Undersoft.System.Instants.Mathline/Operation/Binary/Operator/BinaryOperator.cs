using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace System.Instants.Mathline
{
    [Serializable]
    public abstract class BinaryOperator
    {
        public abstract double Apply(double a, double b);
        public abstract void Compile(ILGenerator g);
    }
}
