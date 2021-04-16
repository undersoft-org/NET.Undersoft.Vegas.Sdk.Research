using System.Collections.Specialized;
using System.Reflection;

namespace System
{
    public interface ISerialNumber<V> : ISerialNumber
    {
        V Value { get; }

        Type IdentifierType { get; }

        FieldInfo[] KeyFields { get; }
    }

    public interface ISerialNumber : IUnique, IEquatable<BitVector32>, IEquatable<DateTime>, IEquatable<ISerialNumber>
    {      
        ushort[] ValueToXYZ(long vectorZ, long vectorY, long value);
        long ValueFromXYZ(int vectorZ, int vectorY);

        ushort BlockZ { get; set; }
        ushort BlockY { get; set; }
        ushort BlockX { get; set; }

        ushort FlagsBlock { get; set; }

        long TimeBlock { get; set; }

    }
}