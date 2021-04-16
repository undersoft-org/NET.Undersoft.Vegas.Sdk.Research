using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;


namespace System.Instants.Mathline
{
    [Serializable]
    public class UnsignedOperator : Formula
    {
        protected Formula e;


        public UnsignedOperator(Formula ee)
        {
            e = ee;
        }       

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            e.Compile(g, cc);
        }

        //public override double Math(int i, int j)
        //{
        //    return e.Math(j, i);
        //}

        public override MathlineSize Size
        {
            get { return e.Size; }
        }
    }
}
