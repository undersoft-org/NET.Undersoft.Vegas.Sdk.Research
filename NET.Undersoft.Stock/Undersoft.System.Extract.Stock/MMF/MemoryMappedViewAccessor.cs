using Microsoft.Win32.SafeHandles;
using System.Extract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Extract.Stock
{
#if !NET40Plus
    /// <summary>
    /// 
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public sealed class MemoryMappedViewAccessor : IDisposable
    {
        MemoryMappedView _view;

        internal MemoryMappedViewAccessor(MemoryMappedView memoryMappedView)
        {
            this._view = memoryMappedView;
        }
        
        /// <summary>
        /// 
        /// </summary>
        public SafeMemoryMappedViewHandle SafeMemoryMappedViewHandle
        {
            [SecurityCritical]
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this._view.SafeMemoryMappedViewHandle;
            }
        }

        /// <summary>
        /// Dispose pattern
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposeManagedResources)
        {
            if (_view != null)
                _view.Dispose();
            _view = null;
        }

        internal static unsafe void PtrToStructure(byte* ptr, object structure)
        {
                Extractor.PointerToStructure(ptr, structure);
        }
        internal static unsafe void PtrToNewStructure(byte* ptr, out object structure, Type t)
        {
            structure = Extractor.PointerToStructure(ptr, t, 0);
        }

        internal static unsafe void StructureToPtr(object structure, byte* ptr)
        {
            Extractor.StructureToPointer(structure, ptr);
        }

        internal unsafe void Write(long position, object structure)
        {
            int elementSize = Marshal.SizeOf(structure.GetType());
            if (position > this._view.Size - elementSize)
                throw new ArgumentOutOfRangeException("position", "");

            try
            {
                byte* ptr = null;
                _view.SafeMemoryMappedViewHandle.AcquireIntPtr(ref ptr);
                ptr += _view.ViewStartOffset;
                StructureToPtr(structure, ptr + position);
            }
            finally
            {
                _view.SafeMemoryMappedViewHandle.ReleaseIntPtr();
            }
        }

        internal unsafe void WriteArray(long position, object[] buffer, int index, int count)
        {
            Type t = null;
            if (buffer != null && buffer.Length > 0)
                t = buffer[0].GetType();
            else
                throw new ArgumentOutOfRangeException("position");

            uint elementSize = (uint)Marshal.SizeOf(t);

            if (position > this._view.Size - (elementSize * count))
                throw new ArgumentOutOfRangeException("position");
            
            try
            {
                byte* ptr = null;
                _view.SafeMemoryMappedViewHandle.AcquireIntPtr(ref ptr);
                ptr += _view.ViewStartOffset + position;

                for (var i = 0; i < count; i++)
                    StructureToPtr(buffer[index + i], ptr + (i * elementSize));
            }
            finally
            {
                _view.SafeMemoryMappedViewHandle.ReleaseIntPtr();
            }
        }

        internal unsafe void Read(long position, object structure, Type t)
        {
            int size = 0;
            bool activateNew = false;
            if (structure != null)
                size = Marshal.SizeOf(structure);
            else
            {
                size = Marshal.SizeOf(t);
                activateNew = true;
            }

            if (position > this._view.Size - size)
                throw new ArgumentOutOfRangeException("position", "");
            try
            {
                byte* ptr = null;
                _view.SafeMemoryMappedViewHandle.AcquireIntPtr(ref ptr);
                ptr += _view.ViewStartOffset;
                if (activateNew)
                    PtrToNewStructure(ptr + position, out structure, t);
                else
                    PtrToStructure(ptr + position, structure);

            }
            finally
            {
                _view.SafeMemoryMappedViewHandle.ReleaseIntPtr();
            }
        }

        internal unsafe void ReadArray(long position, object[] buffer, int index, int count, Type t)
        {
            uint elementSize = 0;
            bool activateNew = false;
            if (buffer == null || buffer.Length < 1)
            {
                buffer = new object[count];
                index = 0;
                elementSize = (uint)Marshal.SizeOf(t);
                activateNew = true;
            }
            else            
                elementSize = (uint)Marshal.SizeOf(buffer[0]);            

            if (position > this._view.Size - (elementSize * count))
                throw new ArgumentOutOfRangeException("position");
            try
            {
                byte* ptr = null;
                _view.SafeMemoryMappedViewHandle.AcquireIntPtr(ref ptr);
                ptr += _view.ViewStartOffset + position;

                if (activateNew)
                    for (var i = 0; i < count; i++)
                        PtrToNewStructure(ptr + (i * elementSize), out buffer[index + i], t);
                else
                    for (var i = 0; i < count; i++)
                        PtrToStructure(ptr + (i * elementSize), buffer[index + i]);

            }
            finally
            {
                _view.SafeMemoryMappedViewHandle.ReleaseIntPtr();
            }
        }
       
    }
#endif
}
