using System.Reflection;

namespace System
{
    public interface IUnique<V> : IUnique
    {
        V Target { get; }

        int[] GetKeyIds(); 
    }


    public interface IUnique : IEquatable<IUnique>, IComparable<IUnique>
    {
        IUnique Empty { get; }

        byte[] GetBytes();

        byte[] GetKeyBytes();

        void SetHashKey(long value);

        long GetHashKey();

        long KeyBlock { get; set; }

    }
}