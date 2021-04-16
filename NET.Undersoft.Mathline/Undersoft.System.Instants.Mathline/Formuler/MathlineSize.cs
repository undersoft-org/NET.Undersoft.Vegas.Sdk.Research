using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Instants.Mathline
{
    [Serializable]
    public class MathlineSize
    {
        public MathlineSize(int i, int j)
        {
            rows = i;
            cols = j;
        }

        public int rows;
        public int cols;

        public static MathlineSize Scalar = new MathlineSize(1, 1);

        public override bool Equals(object o)
        {
            if (o is MathlineSize) return ((MathlineSize)o) == this;
            return false;
        }

        public static bool operator !=(MathlineSize o1, MathlineSize o2)
        {
            return o1.rows != o2.rows || o1.cols != o2.cols;
        }

        public static bool operator ==(MathlineSize o1, MathlineSize o2)
        {
            return o1.rows == o2.rows && o1.cols == o2.cols;
        }

        public override int GetHashCode()
        {
            return rows * cols;
        }

        public override string ToString()
        {
            return "" + rows + " " + cols;
        }
    }
}
