using System.Linq;

namespace System.Collections.Generic
{
    [Serializable]
    public class HashCode32Comparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return x - y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }

    [Serializable]
    public class HashCode32Equality : IEqualityComparer<int>
    {
        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj;
        }
    }

    [Serializable]
    public class HashCode64Equality : IEqualityComparer<long>
    {
        public bool Equals(long x, long y)
        {
            return x == y;
        }

        public unsafe int GetHashCode(long obj)
        {
            unchecked
            {
                byte* pkey = ((byte*)&obj);
                return (((17 + *(int*)(pkey + 4)) * 23) + *(int*)(pkey)) * 23;
            }
        }
    }

    [Serializable]
    public class IntArrayEquality : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            return x.SequenceEqual(y);
        }
        public int GetHashCode(int[] obj)
        {
            unchecked
            {
                return obj.Select(o => o).Aggregate(17, (a, b) => (a + b) * 23); ;
            }
        }
    }

    [Serializable]
    public class ParellelHashCode32Equality : IEqualityComparer<ParallelQuery<IEnumerable<int>>>
    {
        public bool Equals(ParallelQuery<IEnumerable<int>> x, ParallelQuery<IEnumerable<int>> y)
        {
            return x.SelectMany(a=>a.Select(b=>b)).SequenceEqual(y.SelectMany(a=>a.Select(b=>b)));
        }

        public int GetHashCode(ParallelQuery<IEnumerable<int>> obj)
        {
            unchecked
            {
                return obj.SelectMany(o => o.Select(x => x)).Aggregate(17, (a, b) => (a + b) * 23);
            }
        }
    }

    [Serializable]
    public class ParellelHashCode64Equality : IEqualityComparer<ParallelQuery<IEnumerable<long>>>
    {
        public bool Equals(ParallelQuery<IEnumerable<long>> x, ParallelQuery<IEnumerable<long>> y)
        {
            return x.SelectMany(a => a.Select(b => b)).SequenceEqual(y.SelectMany(a => a.Select(b => b)));
        }

        public int GetHashCode(ParallelQuery<IEnumerable<long>> obj)
        {
            unchecked
            {
                return obj.SelectMany(o => o.Select(x => x)).Aggregate(17, (a, b) => (a + (int)b) * 23);
            }
        }
    }

}
