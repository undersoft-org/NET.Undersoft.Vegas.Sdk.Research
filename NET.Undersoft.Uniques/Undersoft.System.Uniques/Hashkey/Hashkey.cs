using System.Collections;
using System.Extract;
using System.Runtime.InteropServices;
using System.Uniques;

namespace System.Uniques
{
    public class Hashkey32 : Hashkey
    {
        public Hashkey32()
        {            
        }

        public override unsafe Byte[] ComputeHash(byte* bytes, int length, uint seed = 0)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b)
            {
                *((ulong*)pb) = xxHash32.UnsafeComputeHash(bytes, length, 0);
            }
            return b;
        }
        public override unsafe Byte[] ComputeHash(byte[] bytes)
        {
            byte[] b = new byte[8];
            fixed (byte* pb = b, pa = bytes)
            {
                *((ulong*)pb) = HashHandle32.ComputeHashCode(pa, bytes.Length, 0);
            }
            return b;
        }
        public override unsafe UInt64 ComputeHashCode(byte* bytes, int length, uint seed = 0)
        {
            return HashHandle32.ComputeHashCode(bytes, length, 0);
        }
        public override unsafe UInt64 ComputeHashCode(byte[] bytes)
        {
            fixed (byte* pa = bytes)
            {
                return HashHandle32.ComputeHashCode(pa, bytes.Length, 0);
            }
        }

        public override Byte[] GetHashBytes(Byte[] obj)
        {
            return obj.GetHashBytes32();
        }
        public override Byte[] GetHashBytes(Object obj)
        {
            return obj.GetHashBytes32();
        }
        public override Byte[] GetHashBytes(IList obj)
        {
            return obj.GetHashBytes32();
        }
        public unsafe override Byte[] GetHashBytes(string obj)
        {
            return obj.GetHashBytes32();
        }
        public override Byte[] GetHashBytes(IUnique obj)
        {
            return obj.GetKeyBytes();
        }

        public override Int64 GetHashKey(Byte[] obj)
        {
            return obj.GetHashKey32();
        }
        public override Int64 GetHashKey(Object obj)
        {
            return obj.GetHashKey32();
        }
        public override Int64 GetHashKey(IList obj)
        {
            return obj.GetHashKey32();
        }
        public override Int64 GetHashKey(string obj)
        {
            return obj.GetHashKey32();
        }
        public override Int64 GetHashKey(IUnique obj)
        {
            return obj.GetHashKey32();
        }
    }

    public class Hashkey64 : Hashkey
    {
        public Hashkey64()
        {
        }

        public override unsafe Byte[] ComputeHash(byte* bytes, int length, uint seed = 0)
        {
            return HashHandle64.ComputeHash(bytes, length, 0);
        }
        public override unsafe Byte[] ComputeHash(byte[] bytes)
        {
            return HashHandle64.ComputeHash(bytes);
        }
        public override unsafe UInt64 ComputeHashCode(byte* bytes, int length, uint seed = 0)
        {
            return HashHandle64.ComputeHashCode(bytes, length, 0);
        }
        public override unsafe UInt64 ComputeHashCode(byte[] bytes)
        {
            return HashHandle64.ComputeHashCode(bytes);
        }

        public override Byte[] GetHashBytes(Byte[] obj)
        {
            return obj.GetHashBytes64();
        }
        public override Byte[] GetHashBytes(Object obj)
        {
            return obj.GetHashBytes64();
        }
        public override Byte[] GetHashBytes(IList obj)
        {
            return obj.GetHashBytes64();
        }
        public override Byte[] GetHashBytes(string obj)
        {
            return obj.GetHashBytes64();
        }
        public override Byte[] GetHashBytes(IUnique obj)
        {
            return obj.GetKeyBytes();
        }

        public override Int64 GetHashKey(Byte[] obj)
        {
            return obj.GetHashKey64();
        }
        public override Int64 GetHashKey(Object obj)
        {
            return obj.GetHashKey64();
        }
        public override Int64 GetHashKey(IList obj)
        {
            return obj.GetHashKey64();
        }
        public override Int64 GetHashKey(string obj)
        {
            return obj.GetHashKey64();
        }
        public override Int64 GetHashKey(IUnique obj)
        {
            return obj.GetHashKey();
        }

    }

    public abstract class Hashkey : IHashkeys
    {
        protected HashBits _bits;

        protected Hashkey _base;

        public Hashkey()
        {            
        }
        public Hashkey(HashBits hashBits)
        {
            _bits = hashBits;
            switch(hashBits)
            {
                case HashBits.bit64:
                    _base = new Hashkey64();
                    break;
                case HashBits.bit32:
                    _base = new Hashkey32();
                    break;
                default:
                    _base = new Hashkey64();
                    break;
            }
            
        }

        public virtual unsafe Byte[] ComputeHash(byte* bytes, int length, uint seed = 0)
        {
            return _base.ComputeHash(bytes, length, seed);
        }
        public virtual unsafe Byte[] ComputeHash(byte[] bytes)
         {
            return _base.ComputeHash(bytes);
        }
        public virtual unsafe UInt64 ComputeHashCode(byte* bytes, int length, uint seed = 0)
        {
            return _base.ComputeHashCode(bytes, length, seed);
        }
        public virtual unsafe UInt64 ComputeHashCode(byte[] bytes)
        {
            return _base.ComputeHashCode(bytes);
        }

        public virtual Byte[] GetHashBytes(Byte[] obj)
        {
            return _base.GetHashBytes(obj);
        }
        public virtual Byte[] GetHashBytes(Object obj)
        {
            return _base.GetHashBytes(obj);
        }
        public virtual Byte[] GetHashBytes(IList obj)
        {
            return _base.GetHashBytes(obj);
        }
        public virtual Byte[] GetHashBytes(string obj)
        {
            return _base.GetHashBytes(obj);
        }
        public virtual Byte[] GetHashBytes(IUnique obj)
        {
            return _base.GetHashBytes(obj);
        }

        public virtual Int64 GetHashKey(Byte[] obj)
        {
            return _base.GetHashKey(obj);
        }
        public virtual Int64 GetHashKey(Object obj)
        {
            return _base.GetHashKey(obj);
        }
        public virtual Int64 GetHashKey(IList obj)
        {
            return _base.GetHashKey(obj);
        }
        public virtual Int64 GetHashKey(string obj)
        {
            return _base.GetHashKey(obj);
        }
        public virtual Int64 GetHashKey(IUnique obj)
        {
            return _base.GetHashKey(obj);
        }

    }

    public interface IHashkeys
    {
        Byte[] GetHashBytes(Byte[] obj);
        Byte[] GetHashBytes(Object obj);
        Byte[] GetHashBytes(IList obj);
        Byte[] GetHashBytes(string obj);
        Byte[] GetHashBytes(IUnique obj);

        Int64 GetHashKey(Byte[] obj);
        Int64 GetHashKey(Object obj);
        Int64 GetHashKey(IList obj);
        Int64 GetHashKey(string obj);
        Int64 GetHashKey(IUnique obj);
    }

    public enum HashBits
    {
        bit64,
        bit32
    }
}

