using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;

namespace System.Extract.Stock.Utilities
{
    [SecurityPermission(SecurityAction.LinkDemand)]
    [SecurityPermission(SecurityAction.InheritanceDemand)]
    public struct StockSlicer : IList, ICollection
    {
        private readonly IList _list;
        private readonly int _offset;
        private readonly int _count;

        public StockSlicer(IList list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            _list = list;
            _offset = 0;
            _count = list.Count;
        }
     
        public StockSlicer(IList list, int offset, int count)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "ArgumentOutOfRange_NeedNonNegNum");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "ArgumentOutOfRange_NeedNonNegNum");
            if (list.Count - offset < count)
                throw new ArgumentException("Argument_InvalidOffLen");

            _list = list;
            _offset = offset;
            _count = count;
        }

        public IList List
        {
            get
            {
                return _list;
            }
        }

        public int Offset
        {
            get
            {
                return _offset;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public override int GetHashCode()
        {
            return null == _list
                        ? 0
                        : _list.GetHashCode() ^ _offset ^ _count;
        }

        public override bool Equals(Object obj)
        {
            if (obj is StockSlicer)
                return Equals((StockSlicer)obj);
            else
                return false;
        }

        public bool Equals(StockSlicer obj)
        {
            return obj._list == _list && obj._offset == _offset && obj._count == _count;
        }

        public static bool operator ==(StockSlicer a, StockSlicer b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(StockSlicer a, StockSlicer b)
        {
            return !(a == b);
        }

        #region IList
        public object this[int index]
        {
            get
            {
                if (_list == null)
                    throw new InvalidOperationException("InvalidOperation_NullArray");
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                return _list[_offset + index];
            }

            set
            {
                if (_list == null)
                    throw new InvalidOperationException("InvalidOperation_NullArray");
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                _list[_offset + index] = value;
            }
        }

        public int IndexOf(object item)
        {
            if (_list == null)
                throw new InvalidOperationException("InvalidOperation_NullArray");

            for (var i = 0; i < Count; i++)
            {
                if (this[i].Equals(item)) return i;
            }
            return -1;
        }

        int IList.Add(object value)
        {
            throw new NotImplementedException();
        }     

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.Insert(int index, object item)
        {
            throw new NotSupportedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        bool IList.Contains(object item)
        {
            if (_list == null)
                throw new InvalidOperationException("InvalidOperation_NullArray");

            return IndexOf(item) >= 0;
        }

        void IList.Clear()
        {
            throw new NotSupportedException();
        }
        #endregion

        #region ICollection
        bool IList.IsReadOnly
        {
            get
            {
                // the indexer setter does not throw an exception although IsReadOnly is true.
                // objecthis is to match the behavior of arrays.
                return true;
            }
        }

        bool IList.IsFixedSize => throw new NotImplementedException();

        int ICollection.Count => throw new NotImplementedException();

        object ICollection.SyncRoot => throw new NotImplementedException();

        bool ICollection.IsSynchronized => throw new NotImplementedException();

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException("InvalidOperation_NullArray");

            return new ArraySliceEnumerator(this);
        }

        IEnumerator GetEnumerator()
        {
            if (_list == null)
                throw new InvalidOperationException("InvalidOperation_NullArray");

            return new ArraySliceEnumerator(this);
        }
        #endregion

        [Serializable]
        private sealed class ArraySliceEnumerator : IEnumerator
        {
            private IList _array;
            private int _start;
            private int _end;
            private int _current;

            internal ArraySliceEnumerator(StockSlicer arraySlice)
            {
                _array = arraySlice._list;
                _start = arraySlice._offset;
                _end = _start + arraySlice._count;
                _current = _start - 1;
            }

            public bool MoveNext()
            {
                if (_current < _end)
                {
                    _current++;
                    return _current < _end;
                }
                return false;
            }

            public object Current
            {
                get
                {
                    if (_current < _start) throw new InvalidOperationException("InvalidOperation_EnumNotStarted");
                    if (_current >= _end) throw new InvalidOperationException("InvalidOperation_EnumEnded");
                    return _array[_current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _current = _start - 1;
            }

            public void Dispose()
            {
            }
        }
    }
}
