using System.Reflection.Emit;

namespace System.Instants.Mathline
{
    [Serializable]
    public abstract class Formula
    {
        //public abstract double Math(int i, int j);     

        public abstract void Compile(ILGenerator g, CompilerContext cc);

        public virtual MathlineSize Size
        {
            get
            {
                return new MathlineSize(0, 0);
            }
        }

        // addition
        public static Formula operator +(Formula e1, Formula e2)
        {
            return new BinaryOperation(e1, e2, new Plus());
        }

        // subtraction
        public static Formula operator -(Formula e1, Formula e2)
        {
            return new BinaryOperation(e1, e2, new Minus());
        }

        // multiplication
        public static Formula operator *(Formula e1, Formula e2)
        {
            return new BinaryOperation(e1, e2, new Multiply());
        }

        // division
        public static Formula operator /(Formula e1, Formula e2)
        {
            return new BinaryOperation(e1, e2, new Divide());            
        }

        // equal
        public static Formula operator ==(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new Equal());
        }

        // not equal
        public static Formula operator !=(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new NotEqual());
        }     

        // lesser
        public static Formula operator <(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new Less());
        }

        // or
        public static Formula operator |(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new OrOperand());
        }

        // greater
        public static Formula operator >(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new Greater());
        }

        // and
        public static Formula operator &(Formula e1, Formula e2)
        {
            return new CompareOperation(e1, e2, new AndOperand());
        }

        // not equal literal
        public static bool operator !=(Formula e1, object o)
        {
            if (o == null)
                return NullCheck.IsNotNull(e1);
            else
                return !e1.Equals(o);
        }

        // equal literal
        public static bool operator ==(Formula e1, object o)
        {
            if (o == null)
                return NullCheck.IsNull(e1);
            else
                return e1.Equals(o);
        }

        // power e2 is always a literal
        public Formula Pow(Formula e2)
        {
            return new PowerOperation(this, e2);
        }

        public static Formula MemPow(Formula e1, Formula e2)
        {
            return new PowerOperation(e1, e2);
        }

        public CombinedFormula Prepare(Formula f, LeftFormula m, bool partial = false)
        {
            CombinedFormula = new CombinedFormula(m, f, partial);
            CombinedFormula.LeftFormula = m;
            CombinedFormula.RightFormula = f;
            return CombinedFormula;
        }
        public CombinedFormula Prepare(LeftFormula m, bool partial = false)
        {
            CombinedFormula = new CombinedFormula(m, this, partial);
            CombinedFormula.LeftFormula = m;
            CombinedFormula.RightFormula = this;
            return CombinedFormula;
        }

        [NonSerialized] public Formula LeftFormula;
        [NonSerialized] public Formula RightFormula;
        [NonSerialized] public CombinedFormula CombinedFormula;

        public void Reckon(CombinedReckoner ev)
        {            
            Reckoner e = new Reckoner(ev.Reckon);
            e();
        }

        public Reckoner GetReckoner(CombinedFormula m)
        {
            CombinedReckoner reckoner = CombineReckoner(m);
            Reckoner ev = new Reckoner(reckoner.Reckon);
            return ev;
        }
        public Reckoner GetReckoner(CombinedReckoner e)
        {
            Reckoner ev = new Reckoner(e.Reckon);
            return ev;
        }
        public Reckoner GetReckoner(Formula f, LeftFormula m)
        {
            CombinedReckoner reckoner = CombineReckoner(f, m);
            Reckoner ev = new Reckoner(reckoner.Reckon);
            return ev;
        }

        public CombinedReckoner CombineReckoner(CombinedFormula m)
        {
            return Compiler.Compile(m);
        }
        public CombinedReckoner CombineReckoner(Formula f, LeftFormula m)
        {
            CombinedReckoner reckoner = Compiler.Compile(new CombinedFormula(m, f));
            return reckoner;
        }     

        public Formula Transpose()
        {
            return new TransposeOperation(this);
        }

        public static implicit operator Formula(double f)
        {
            return new UnsignedFormula(f);
        }

        public static Formula Cos(Formula e)
        {
            return new FunctionOperation(e, FunctionOperation.FunctionType.Cos);
        }
        public static Formula Sin(Formula e)
        {
            return new FunctionOperation(e, FunctionOperation.FunctionType.Sin);
        }
        public static Formula Log(Formula e)
        {
            return new FunctionOperation(e, FunctionOperation.FunctionType.Log);
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (o == null)
                return false;
            return this.Equals(o);
        }
    }

    public static class NullCheck
    {
        public static bool IsNotNull(object o)
        {
            if (o is ValueType)
                return false;
            else
                return !ReferenceEquals(o, null);
        }
        public static bool IsNull(object o)
        {
            if (o is ValueType)
                return false;
            else
                return ReferenceEquals(o, null);
        }
    }

    public class SizeMismatchException : Exception
    {
        public SizeMismatchException(string s) : base(s)
        {

        }
    }
}
