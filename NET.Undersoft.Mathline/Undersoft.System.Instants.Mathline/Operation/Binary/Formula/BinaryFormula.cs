using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace System.Instants.Mathline
{
    [Serializable]
    public abstract class BinaryFormula : Formula
    {
        public BinaryFormula(Formula e1, Formula e2)
        {
            expr1 = e1;
            expr2 = e2;
        }

        protected Formula expr1, expr2;
    }
}
