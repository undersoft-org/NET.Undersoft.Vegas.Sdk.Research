using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Extract;

namespace System.Extract.Stock
{
    [SecurityPermission(SecurityAction.LinkDemand)]
    [SecurityPermission(SecurityAction.InheritanceDemand)]
    public unsafe class StockArray : StockHandle, IList<object>, IStock
    {
        public int Length
        { get; private set; }

        public object this[int index]
        {
            get
            {
                object item = null;
                Read(item, index, type);
                return item;
            }
            set
            {
                Write(value, index, type);
            }
        }
              
        public object this[int index, int field, Type t]
        {
            get
            {
                object item = null;
                Read(item, index, t, 1000);
                return item;
            }
            set
            {
                Write(value, index, t, 1000);
            }
        }

        public void Rewrite(int index, object structure)
        {
            Read(structure, index);
        }

        private int _elementSize;
        private Type type;

        public StockArray(string file, string name, int length, Type t) :
                          base(file, name, Marshal.SizeOf(t) * length, true, true)
        {
            type = t;
            Length = length;
            _elementSize = Marshal.SizeOf(t);

            Open();
        }                   // Creates or Open as Prime
        public StockArray(string file, string name, int length, int size) :
                         base(file, name, size * length, true, true)
        {
            type = typeof(byte[]);
            Length = length;
            _elementSize = size;

            Open();
        }                   // Creates or Open as Prime
        public StockArray(string file, string name, Type t) :
                          base(file, name, 0, false, true)
        {
            type = t;
            _elementSize = Marshal.SizeOf(t);

            Open();
        }                               // Open as User

        protected override bool DoOpen()
        {
            if (!IsOwnerOfSharedMemory)
            {
                if (BufferSize % _elementSize != 0)
                    throw new ArgumentOutOfRangeException("name", "BufferSize is not evenly divisible by the size of " + type.Name);

                Length = (int)(BufferSize / _elementSize);
            }
            return true;
        }

        #region Writing

        new public void Write(object data, long position = 0, Type t = null, int timeout = 1000)
        {
            if (position > Length - 1 || position < 0)
                throw new ArgumentOutOfRangeException("index");
            if (t == null)
                t = type;
            base.Write(data, position * _elementSize, t, timeout);
        }
        new public void Write(object[] buffer, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length + position > Length || position < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            base.Write(buffer, position * _elementSize, t, timeout);
        }
        new public void Write(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            if (buffer == null)
                throw new ArgumentNullException("buffer");
            if (buffer.Length - index < count)
                count = buffer.Length - index;
            if (count + position > Length || position < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            base.Write(buffer, index, count, position * _elementSize, t, timeout);
        }
        new public void Write(IntPtr ptr, long length, long position = 0, Type t = null, int timeout = 1000)
        {
            base.Write(ptr, length, (position * _elementSize), t, timeout);
        }
        new public void Write(byte* ptr, long length, long position = 0, Type t = null, int timeout = 1000)
        {
            base.Write(ptr, length, (position * _elementSize), t, timeout);
        }

        #endregion

        #region Reading

        new public void Read(object data, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            if (position > Length - 1 || position < 0)
                throw new ArgumentOutOfRangeException("index");

            base.Read(data, (position * _elementSize), t, timeout);
        }
        new public void Read(object[] buffer, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            if (buffer == null)
                throw new ArgumentOutOfRangeException("buffer");
            if (Length - position < 0 || position < 0)
                position = 0;

            if (buffer.Length + position > Length || position < 0)
                throw new ArgumentOutOfRangeException("index");

            base.Read(buffer, position * _elementSize, t, timeout);
        }
        new public void Read(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            if (buffer == null)
                throw new ArgumentOutOfRangeException("buffer");
            if (Length - position < 0 || position < 0)
                position = 0;

            if (buffer.Length - index < count)
                count = buffer.Length - index;

            if (count + position > Length || position < 0)
                throw new ArgumentOutOfRangeException("index");

            base.Read(buffer,index, count, position * _elementSize, t, timeout);
        }
        new public void Read(IntPtr destination, long length, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            base.Read(destination, length, (position * _elementSize), t, timeout);
        }
        new public void Read(byte* destination, long length, long position = 0, Type t = null, int timeout = 1000)
        {
            if (t == null)
                t = type;
            base.Read(destination, length, (position * _elementSize), t, timeout);
        }

        public void CopyTo(object[] buffer, int position = 0)
        {
            if (buffer == null)
            {
                if (Length - position < 0 || position < 0)
                    position = 0;
                buffer = new object[Length - position];
            }
            if (buffer.Length + position > Length || position < 0)
                throw new ArgumentOutOfRangeException("startIndex");

            base.Read(buffer, position * _elementSize);
        }

        public void CopyTo(IStock destination, uint length, int position = 0)
        {
            Extractor.CopyBlock(destination.GetStockPtr() + position, this.GetStockPtr(), length);
        }
        #endregion

        #region IEnumerable<T>

        public IEnumerator<object> GetEnumerator()
        {
            for (int i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IList<object>

        public void Add(object item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object item)
        {
            return IndexOf(item) >= 0;
        }

        public bool Remove(object item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return Length; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int IndexOf(object item)
        {
            for (var i = 0; i < Count; i++)
            {
                if (this[i].Equals(item)) return i;
            }
            return -1;
        }

        public void Insert(int index, object item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
