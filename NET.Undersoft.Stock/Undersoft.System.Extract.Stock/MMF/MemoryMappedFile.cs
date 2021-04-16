using System;
using System.IO;
using System.Security;
using Microsoft.Win32.SafeHandles;
using System.Security.Permissions;
using System.Extract.Stock;
using System.Runtime.InteropServices;

namespace System.Extract.Stock
{
    /// <summary>
    /// <para>Very limited .NET 3.5 implementation of a managed wrapper around memory-mapped files to reflect the .NET 4 API.</para>
    /// <para>Only those methods and features necessary for the Stock library have been implemented.</para>
    /// </summary>
    [SecurityPermission(SecurityAction.LinkDemand)]
    public sealed class MemoryMappedFile: IDisposable
    {
        SafeMemoryMappedFileHandle _handle;

        public SafeMemoryMappedFileHandle SafeMemoryMappedFileHandle
        {
            [SecurityCritical]
            [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                return this._handle;
            }
        }

        private MemoryMappedFile(SafeMemoryMappedFileHandle handle)
        {
            this._handle = handle;
        }

        ~MemoryMappedFile()
        {
            this.Dispose(false);
        }

        public static MemoryMappedFile CreateNew(String path, String mapName, long capacity, out bool exists, bool readCapacity = false)
        {
            if (String.IsNullOrEmpty(mapName) || String.IsNullOrEmpty(path))
                throw new ArgumentException("mapName cannot be null or empty.");

            if (capacity <= 0)
                throw new ArgumentOutOfRangeException("capacity", "Value must be larger than 0.");

            if (IntPtr.Size == 4 && capacity > ((1024*1024*1024) * (long)4))
                throw new ArgumentOutOfRangeException("capacity", "The capacity cannot be greater than the size of the system's logical address space.");

            return new MemoryMappedFile(DoCreate(path, mapName, capacity, out exists));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Interoperability", "CA1404:CallGetLastErrorImmediatelyAfterPInvoke"), SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        private static SafeMemoryMappedFileHandle DoCreate(string path, string mapName, long capacity, out bool exists, bool readCapacity = false)
        {
            exists = File.Exists(path);           
            FileStream file = File.Open(path, FileMode.OpenOrCreate);
            if (exists && file.Length != capacity)
            {
                if (!readCapacity)
                {
                    file.Close();
                    File.Copy(path, path + "_old", true);
                    File.Delete(path);
                    file = File.Open(path, FileMode.OpenOrCreate);
                }
                else
                {
                    byte[] byt = new byte[8];
                    file.Read(byt, 0, 8);
                    capacity = BitConverter.ToInt32(byt, 0);
                }
            }
            SafeFileHandle fileHandle = file.SafeFileHandle;
            SafeMemoryMappedFileHandle safeHandle = UnsafeNative.CreateFileMapping(fileHandle,
                                                   (UnsafeNative.FileMapProtection)MemoryMappedFileAccess.ReadWrite,
                                                   capacity, mapName);

            var lastWin32Error = Marshal.GetLastWin32Error();
            if (!safeHandle.IsInvalid && (lastWin32Error == UnsafeNative.ERROR_ALREADY_EXISTS))
                throw new System.IO.IOException(UnsafeNative.GetMessage(lastWin32Error));
            else if (safeHandle.IsInvalid && lastWin32Error > 0)
                throw new System.IO.IOException(UnsafeNative.GetMessage(lastWin32Error));

            if (safeHandle == null || safeHandle.IsInvalid)
                throw new InvalidOperationException("Cannot create file mapping");

            return safeHandle;
        }

        [SecurityCritical]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public MemoryMappedViewAccessor CreateViewAccessor(long offset, long size, MemoryMappedFileAccess access = MemoryMappedFileAccess.ReadWrite)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset", "Value must be non-negative");
            if (size < 0)
                throw new ArgumentOutOfRangeException("size", "Value must be positive or zero for default size");
            if (IntPtr.Size == 4 && size > ((1024 * 1024 * 1024) * (long)4))
                throw new ArgumentOutOfRangeException("size", "The capacity cannot be greater than the size of the system's logical address space.");
            MemoryMappedView memoryMappedView = MemoryMappedView.CreateView(this._handle, access, offset, size);
            return new MemoryMappedViewAccessor(memoryMappedView);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposeManagedResources)
        {
            if (this._handle != null && !this._handle.IsClosed)
            {
                this._handle.Dispose();
                this._handle = null;
            }
        }

        public static MemoryMappedFile OpenExisting(string mapName)
        {
            SafeMemoryMappedFileHandle safeMemoryMappedFileHandle = UnsafeNative.OpenFileMapping((uint)MemoryMappedFileRights.ReadWrite, false, mapName);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (safeMemoryMappedFileHandle.IsInvalid)
            {
                if (lastWin32Error == UnsafeNative.ERROR_FILE_NOT_FOUND)
                    throw new FileNotFoundException();
                throw new System.IO.IOException(UnsafeNative.GetMessage(lastWin32Error));
            }
            return new MemoryMappedFile(safeMemoryMappedFileHandle);
        }
    }
}
