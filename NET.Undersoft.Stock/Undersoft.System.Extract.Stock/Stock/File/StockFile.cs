using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Extract;

namespace System.Extract.Stock
{
    /// <summary>
    /// Read/Write buffer with support for simple inter-process read/write synchronisation.
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public unsafe class StockFile : StockHandle, IStock
    {
        public Type type;

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
        public object this[int index, int offset, Type t]
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
            Read(structure, index, type);
        }

        public StockFile(string file, string name, int bufferSize, Type _type) : 
                         base(file, name, bufferSize, true, true)
        {
            type = _type;
            Open();
        }
        public StockFile(string file, string name, Type _type) : 
                         base(file, name, 0, false, true)
        {
            type = _type;
            Open();
        }       

        #region Writing

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(object data, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Write(data, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(object[] data, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Write(data, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Write(buffer, index, count, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(IntPtr ptr, long length, long position = 0, Type t = null, int timeout = 1000)
        {
            AcquireWriteLock();
            base.Write(ptr, length, position);
            ReleaseWriteLock();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(Action<IntPtr> writeFunc, long position = 0)
        {

            base.Write(writeFunc, position);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Write(byte* ptr, long length, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Write(ptr, length, position);

        }

        #endregion

        #region Reading

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(object data, long position = 0, Type t = null, int timeout = 1000)
        {
            base.Read(data, position, t);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(object[] data, long position = 0, Type t = null, int timeout = 1000)
        {
            base.Read( data, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000)
        {
            AcquireReadLock();
            base.Read(buffer, index, count, position, t, timeout);
            ReleaseReadLock();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(IntPtr destination, long length, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Read(destination, length, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(byte* destination, long length, long position = 0, Type t = null, int timeout = 1000)
        {

            base.Read(destination, length, position, t, timeout);

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods")]
        new public void Read(Action<IntPtr> readFunc, long position = 0)
        {

            base.Read(readFunc, position);

        }

        #endregion

        public void CopyTo(IStock destination, uint length, int startIndex = 0)
        {
            Extractor.CopyBlock(destination.GetStockPtr() + startIndex, this.GetStockPtr(), length);
        }
    }
}
