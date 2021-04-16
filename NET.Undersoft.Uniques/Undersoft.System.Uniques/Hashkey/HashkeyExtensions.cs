using System.Collections;
using System.Extract;
using System.Linq;
using System.Runtime.InteropServices;

namespace System.Uniques
{
    public static class HashHandle64
    {
        public static unsafe Byte[] ComputeHash(byte[] bytes)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b, pa = bytes)
            {
                *((ulong*)pb) = xxHash64.UnsafeComputeHash(pa, bytes.Length, 0);
            }
            return b;
        }
        public static unsafe Byte[] ComputeHash(byte* bytes, int length, uint seed = 0)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b)
            {
                *((ulong*)pb) = xxHash64.UnsafeComputeHash(bytes, length, seed);
            }
            return b;
        }
        public static unsafe ulong ComputeHashCode(byte[] bytes)
        {
            fixed (byte* pa = bytes)
            {
                return xxHash64.UnsafeComputeHash(pa, bytes.Length, 0);
            }
        }
        public static unsafe ulong ComputeHashCode(byte* ptr, int length, ulong seed = 0)
        {
            return xxHash64.UnsafeComputeHash(ptr, length, seed);
        }
    }

    public static class HashHandle32
    {
        public static unsafe Byte[] ComputeHash(byte[] bytes)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b, pa = bytes)
            {
                *((uint*)pb) = xxHash32.UnsafeComputeHash(pa, bytes.Length, 0);
            }
            return b;
        }
        public static unsafe Byte[] ComputeHash(byte* ptr, int length, uint seed = 0)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b)
            {
                *((uint*)pb) = xxHash32.UnsafeComputeHash(ptr, length, seed);
            }
            return b;
        }
        public static unsafe ulong ComputeHashCode(byte[] bytes)
        {
            fixed (byte* pa = bytes)
            {
                return xxHash32.UnsafeComputeHash(pa, bytes.Length, 0);
            }
        }
        public static unsafe uint ComputeHashCode(byte* ptr, int length, uint seed = 0)
        {
            return xxHash32.UnsafeComputeHash(ptr, length, seed);
        }
    }

    public unsafe static class HashkeyExtensions64
    {
        public static Byte[] GetHashBytes64(this Byte[] bytes)
        {
            return HashHandle64.ComputeHash(bytes);
        }
        public static Byte[] GetHashBytes64(this Object obj)
        {
            if (obj is ValueType)
                return GetValueTypeHashBytes64(obj);
            if (obj is string)
                return (((string)obj)).GetHashBytes64();
            if (obj is IList)
                return GetHashBytes64((IList)obj);
            return HashHandle64.ComputeHash(obj.GetBytes());
        }
        public static Byte[] GetHashBytes64(this IList obj)
        {
            return HashHandle64.ComputeHash(obj.GetBytes());
        }
        public static Byte[] GetHashBytes64(this String obj)
        {
            fixed (char* c = obj)
                return HashHandle64.ComputeHash((byte*)c, obj.Length * sizeof(char), 0);
        }
        public static Byte[] GetHashBytes64(this IUnique obj)
        {
            return obj.GetKeyBytes();
        }

        public static Int64 GetHashKey64(this Byte[] bytes)
        {
            return (long)HashHandle64.ComputeHashCode(bytes);
        }
        public static Int64 GetHashKey64(this Object obj)
        {
            if (obj is ValueType)
                return GetValueTypeHashKey64(obj);
            if(obj is IUnique)
                return ((IUnique)obj).KeyBlock;
            if (obj is string)
                return (((string)obj)).GetHashKey64();
            if (obj is IList)
                return GetHashKey64((IList)obj);
            return (long)HashHandle64.ComputeHashCode(obj.GetBytes());
        }
        public static Int64 GetHashKey64(this IList obj)
        {
            if (obj is Byte[])
                return GetHashKey64((Byte[])obj);
            return (long)HashHandle64.ComputeHashCode(obj.GetBytes());
        }
        public static Int64 GetHashKey64(this String obj)
        {
            fixed (char* c = obj)
            {
                return (long)HashHandle64.ComputeHashCode((byte*)c, obj.Length * sizeof(char), 0);
            }
        }
        public static Int64 GetHashKey64(this IUnique obj)
        {
            return obj.KeyBlock;
        }

        public static Int32 GetHashCode<T>(this IEquatable<T> obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this Byte[] obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this Object obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this IList obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this string obj)
        {
            return obj.GetHashKey32();
        }

        public static Int64 GetHashKey<T>(this IEquatable<T> obj)
        {
            return obj.GetHashKey64();
        }
        public static Int64 GetHashKey(this Byte[] bytes)
        {
            return bytes.GetHashKey64();
        }
        public static Int64 GetHashKey(this Object obj)
        {
            return obj.GetHashKey64();
        }
        public static Int64 GetHashKey(this IList obj)
        {
            return obj.GetHashKey64();
        }
        public static Int64 GetHashKey(this String obj)
        {
            return obj.GetHashKey64();
        }
        public static Int64 GetHashKey(this IUnique obj)
        {
            return obj.KeyBlock;
        }

        public static bool IsSameOrNull(this Object obj, Object value)
        {
            if (obj != null)
                return obj.Equals(value);
            return (obj == null && value == null);
        }
        public static Int64 ComparableInt64(this Object obj, Type type = null)
        {
            if (type == null)
                type = obj.GetType();

            if (obj is string)
            {
                if (type != typeof(string))
                {
                    if (type == typeof(IUnique))
                        return new Usid((string)obj).KeyBlock;
                    if (type == typeof(DateTime))
                        return ((DateTime)Convert.ChangeType(obj, type)).ToBinary();
                    return Convert.ToInt64(Convert.ChangeType(obj, type));
                }
                return ((string)obj).GetHashKey64();
            }

            if (type == typeof(IUnique))
                return ((IUnique)obj).KeyBlock;
            if (type == typeof(DateTime))
                return ((DateTime)obj).Ticks;
            if (obj is ValueType)
                return Convert.ToInt64(obj);
            return obj.GetHashKey64();
        }
        public static Double ComparableDouble(this Object obj, Type type = null)
        {
            if (type == null)
                type = obj.GetType();

            if (obj is string)
            {
                if (type != typeof(string))
                {
                    if (type == typeof(Usid))
                        return new Usid((string)obj).KeyBlock;
                    if (type == typeof(DateTime))
                        return ((DateTime)Convert.ChangeType(obj, type)).ToOADate();
                    return Convert.ToDouble(Convert.ChangeType(obj, type));
                }
                return (((string)obj).GetHashKey64());
            }

            if (type == typeof(IUnique))
                return (((IUnique)obj).KeyBlock);
            if (type == typeof(DateTime))
                return ((DateTime)obj).ToOADate();
            if (obj is ValueType)
                return Convert.ToDouble(obj);
            return (obj.GetHashKey64());
        }

        public static Int64 GetValueTypeHashKey64(object obj)
        {
            if (obj is IUnique)
                return ((IUnique)obj).KeyBlock;

            Type t = obj.GetType();
            if (t.IsPrimitive)
            {
                byte* ps = stackalloc byte[8];
                Marshal.StructureToPtr(obj, (IntPtr)ps, false);
                return *(long*)ps;
            }
            
            if (obj is DateTime)
                return ((DateTime)obj).ToBinary();
            if (obj is Enum)
                return Convert.ToInt32(obj);

            if (t.IsLayoutSequential)
            {
                byte* ps = stackalloc byte[8];
                ExtractOperation.ValueStructureToPointer(obj, ps, 0);
                return *(long*)ps;
            }


            return (long)HashHandle64.ComputeHashCode(obj.GetBytes());
        }

        public static Byte[] GetValueTypeHashBytes64(object obj)
        {
            if (obj is IUnique)
                return ((IUnique)obj).GetKeyBytes();

            if (obj.GetType().IsPrimitive)
            {
                byte[] s = new byte[8];
                fixed (byte* ps = s)
                {
                    Marshal.StructureToPtr(obj, (IntPtr)ps, false);
                    return s;
                }
            }
           
            if (obj is DateTime)
                return ((DateTime)obj).ToBinary().GetBytes();
            if (obj is Enum)
                return Convert.ToInt32(obj).GetBytes();

            if (obj.GetType().IsLayoutSequential)
            {
                byte[] s = new byte[8];
                fixed (byte* ps = s)
                {
                    ExtractOperation.ValueStructureToPointer(obj, ps, 0);
                    return s;
                }
            }

            return ((long)HashHandle64.ComputeHashCode(obj.GetBytes())).GetBytes();
        }

    }

    public unsafe static class HashkeyExtensions32
    {
        public static Int32 BitAggregate64to32(byte* bytes)
        {
            return new int[] { *((int*)&bytes), *((int*)&bytes[4]) }
                                       .Aggregate(7, (a, b) => (a + b) * 23);

        }
        public static Byte[] BitAggregate64to32(this Byte[] bytes)
        {
            byte[] bytes32 = new byte[4];
            fixed (byte* h32 = bytes32)
            fixed (byte* h64 = bytes)
            {
                *((int*)h32) = new int[] { *((int*)&h64), *((int*)&h64[4]) }
                                           .Aggregate(7, (a, b) => (a + b) * 23);
                return bytes32;
            }
        }
        public static Byte[] BitAggregate32to16(this Byte[] bytes)
        {
            byte[] bytes16 = new byte[2];
            fixed (byte* h16 = bytes16)
            fixed (byte* h32 = bytes)
            {
                *((short*)h16) = new short[] { *((short*)&h32), *((short*)&h32[2]) }
                                               .Aggregate((short)7, (a, b) => (short)((a + b) * 7));
                return bytes16;
            }
        }
        public static Byte[] BitAggregate64to16(this Byte[] bytes)
        {
            byte[] bytes16 = new byte[2];
            fixed (byte* h16 = bytes16)
            fixed (byte* h64 = bytes)
            {
                *((short*)h16) = new short[] { *((short*)&h64), *((short*)&h64[2]),
                                               *((short*)&h64[4]), *((short*)&h64[6]) }
                                               .Aggregate((short)7, (a, b) => (short)((a + b) * 7));
                return bytes16;
            }
        }

        public static Byte[] GetHashBytes32(this Byte[] bytes)
        {
            return HashHandle32.ComputeHash(bytes);
        }
        public static Byte[] GetHashBytes32(this Object obj)
        {
            if (obj is ValueType)
                return GetValueTypeHashBytes32(obj);
            if (obj is string)
                return (((string)obj)).GetHashBytes32();
            if (obj is IList)
                return GetHashBytes32((IList)obj);
            return HashHandle32.ComputeHash(obj.GetBytes());
        }
        public static Byte[] GetHashBytes32(this IList obj)
        {
            return HashHandle32.ComputeHash(obj.GetBytes());
        }
        public static Byte[] GetHashBytes32(this String obj)
        {
            fixed (char* c = obj)
                return HashHandle32.ComputeHash((byte*)c, obj.Length * sizeof(char), 0);
        }
        public static Byte[] GetHashBytes32(this IUnique obj)
        {
            return obj.GetKeyBytes().BitAggregate64to32();
        }

        public static Int32 GetHashKey32(this Byte[] obj)
        {
            return (int)HashHandle32.ComputeHashCode(obj);
        }
        public static Int32 GetHashKey32(this Object obj)
        {
            if (obj is ValueType)
                return GetValueTypeHashKey32(obj);
            if (obj is IUnique)
                return ((IUnique)obj).GetHashKey32();
            if (obj is string)
                return (((string)obj)).GetHashKey32();
            if (obj is IList)
                return GetHashKey32((IList)obj);
            return (int)HashHandle32.ComputeHashCode(obj.GetBytes());
        }
        public static Int32 GetHashKey32(this IList obj)
        {
            if (obj is Byte[])
                return GetHashKey32((Byte[])obj);
            return (int)HashHandle32.ComputeHashCode(obj.GetBytes());
        }
        public static Int32 GetHashKey32(this string obj)
        {
            fixed (char* c = obj)
                return (int)HashHandle32.ComputeHashCode((byte*)c, obj.Length * sizeof(char), 0);
        }
        public static Int32 GetHashKey32(this IUnique obj)
        {
            return obj.GetHashBytes32().ToInt32();
        }

        public static Int32 GetHashCode<T>(this IEquatable<T> obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this Byte[] obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this Object obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this IList obj)
        {
            return obj.GetHashKey32();
        }
        public static Int32 GetHashCode(this string obj)
        {
            return obj.GetHashKey32();
        }

        public static Int32 GetValueTypeHashKey32(object obj)
        {
            if (obj is IUnique)
                return ((IUnique)obj).GetKeyBytes().BitAggregate64to32().ToInt32();

            if (obj.GetType().IsPrimitive)
            {
                byte* ps = stackalloc byte[8];
                Marshal.StructureToPtr(obj, (IntPtr)ps, false);
                return BitAggregate64to32(ps);
            }
          
            if (obj is DateTime)
                return ((DateTime)obj).ToBinary().GetBytes().BitAggregate64to32().ToInt32();
            if (obj is Enum)
                return Convert.ToInt32(obj);

            if (obj.GetType().IsLayoutSequential)
            {
                byte* ps = stackalloc byte[8];
                ExtractOperation.ValueStructureToPointer(obj, ps, 0);
                return BitAggregate64to32(ps);
            }


            return (int)HashHandle32.ComputeHashCode(obj.GetBytes());
        }

        public static Byte[] GetValueTypeHashBytes32(object obj)
        {
            if (obj is IUnique)
                return ((IUnique)obj).GetKeyBytes().BitAggregate64to32();

            if (obj.GetType().IsPrimitive)
            {
                byte[] s = new byte[8];
                fixed (byte* ps = s)
                {
                    Marshal.StructureToPtr(obj, (IntPtr)ps, false);
                    return s.BitAggregate64to32();
                }
            }
           
            if (obj is DateTime)
                return ((DateTime)obj).ToBinary().GetBytes().BitAggregate64to32();

            if (obj is Enum)
                return Convert.ToInt32(obj).GetBytes();

            if (obj.GetType().IsLayoutSequential)
            {
                byte[] s = new byte[8];
                fixed (byte* ps = s)
                {
                    ExtractOperation.ValueStructureToPointer(obj, ps, 0);
                    return s.BitAggregate64to32();
                }
            }

            return ((int)HashHandle32.ComputeHashCode(obj.GetBytes())).GetBytes();
        }
    }

}
