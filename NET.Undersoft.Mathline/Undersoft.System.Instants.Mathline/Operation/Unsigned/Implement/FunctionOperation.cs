using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace System.Instants.Mathline
{
    [Serializable]
    public class FunctionOperation : UnsignedOperator
    {
        public enum FunctionType { Cos, Sin, Ln, Log };
        FunctionType effx;

        public FunctionOperation(Formula ee, FunctionType fx) : base(ee)
        {
            effx = fx;
        }

        public override void Compile(ILGenerator g, CompilerContext cc)
        {
            if (cc.IsFirstPass())
            {
                e.Compile(g, cc);
                return;
            }
            MethodInfo mi = null;

            switch (effx)
            {                
                case FunctionType.Cos: mi = typeof(Math).GetMethod("Cos"); break;
                case FunctionType.Sin: mi = typeof(Math).GetMethod("Sin"); break;
                case FunctionType.Ln: mi = typeof(Math).GetMethod("Log"); break;
                case FunctionType.Log: mi = typeof(Math).GetMethod("Log10"); break;
                default:
                    break;
            }
            if (mi == null) return;

            e.Compile(g, cc);

            g.EmitCall(OpCodes.Call, mi, null);
        }

        //public override double Math(int i, int j)
        //{
        //    double f = e.Math(i, j);
        //    switch (effx)
        //    {
        //        case FunctionType.Cos: return Math.Cos(f);
        //        case FunctionType.Sin: return Math.Sin(f);
        //        case FunctionType.Ln: return Math.Log(f);
        //        case FunctionType.Log: return Math.Log10(f);
        //        default:
        //            return f;
        //    }
        //}

        public override MathlineSize Size
        {
            get { return e.Size; }
        }
    }
}
