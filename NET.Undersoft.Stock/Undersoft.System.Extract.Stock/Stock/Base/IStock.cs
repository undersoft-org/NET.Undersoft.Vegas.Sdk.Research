using System;
using System.Extract;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Diagnostics;
using System.Threading;

namespace System.Extract.Stock
{   
    public interface IStock
    {
        object this[int index] { get; set; }
        object this[int index, int field, Type t] { get; set; }

        void Rewrite(int index, object structure);

        bool Exists
        { get; set; } 
        bool FixSize
        { get;  set; }

        string Path
        { get; set; }

        ushort StockId
        { get; set; }
        ushort SectorId
        { get; set; }

        long BufferSize
        { get; set; }
        long UsedSize
        { get; set; }
        long FreeSize
        { get; set; }

        int ItemSize
        { get; set; }
        int ItemCount
        { get; set; }
        long ItemCapacity
        { get; set; }

        long SharedMemorySize
        {
            get;            
        }
        long UsedMemorySize
        {
            get;
        }
        long FreeMemorySize
        {
            get;
        }

        void ReadHeader();

        void WriteHeader();

        IntPtr GetStockPtr();

        void CopyTo(IStock destination, uint length, int startIndex = 0);

        void Write(object data, long position = 0, Type t = null, int timeout = 1000);
        void Write(object[] buffer, long position = 0, Type t = null, int timeout = 1000);
        void Write(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000);
        void Write(IntPtr source, long length, long position = 0, Type t = null, int timeout = 1000);
        unsafe void Write(byte* source, long length, long position = 0, Type t = null, int timeout = 1000);

        void Read(object data, long position = 0, Type t = null, int timeout = 1000);
        void Read(object[] buffer, long position = 0, Type t = null, int timeout = 1000);
        void Read(object[] buffer, int index, int count, long position = 0, Type t = null, int timeout = 1000);
        void Read(IntPtr destination, long length, long position = 0, Type t = null, int timeout = 1000);
        unsafe void Read(byte* destination, long length, long position = 0, Type t = null, int timeout = 1000);

        //void EnumMemory(EnumMemoryDelegate enumMemoryCallback);

        //string GetMappedFileName(IntPtr address);

        void Close();

        void Dispose();
    }
}

// Below Win32 Memory Searching for any Process with Map File Ownership - doesn't work on Linux and Docker Linux Get Current Process Method
// not found and Win32 library in NETCore for linux throw Exception.  
//
//public delegate bool EnumMemoryDelegate(MemoryBasicInformation info);
//public void EnumMemory(EnumMemoryDelegate enumMemoryCallback)
//{
//    IntPtr address = IntPtr.Zero;
//    MemoryBasicInformation mbi = new MemoryBasicInformation();
//    int mbiSize = Marshal.SizeOf(mbi);

//    while (Win32.VirtualQueryEx(Process.GetCurrentProcess().Handle, address, out mbi, mbiSize) != 0)
//    {
//        if (!enumMemoryCallback(mbi))
//            break;

//        address = address.Increment(mbi.RegionSize);
//    }
//}

//public bool CheckMapFileOwnership(string path)
//{
//    bool hasOwner = false;
//    string currentpath = !path.Contains(":") ? Directory.GetCurrentDirectory() + "/" + path : path;
//    string unipath = currentpath.Replace("/", "\\");

//    EnumMemory((region) =>
//    {
//        if (region.Type != MemoryType.Mapped)
//            return true;

//        if (unipath.Equals(GetMappedFileName(region.BaseAddress)))
//            hasOwner = true;

//        return true;
//    });

//    return hasOwner;
//}

//public string GetMappedFileName(IntPtr address)
//{
//    NtStatus status;
//    IntPtr retLength;

//    using (var data = new MemoryAlloc(20))
//    {
//        if ((status = Win32.NtQueryVirtualMemory(
//            Process.GetCurrentProcess().Handle,
//            address,
//            MemoryInformationClass.MemoryMappedFilenameInformation,
//            data.Memory,
//            data.Size.ToPointer(),
//            out retLength
//            )) == NtStatus.BufferOverflow)
//        {
//            data.ResizeNew(retLength.ToInt32());

//            status = Win32.NtQueryVirtualMemory(
//                Process.GetCurrentProcess().Handle,
//                address,
//                MemoryInformationClass.MemoryMappedFilenameInformation,
//                data.Memory,
//                data.Size.ToPointer(),
//                out retLength
//                );
//        }

//        if (status >= NtStatus.Error)
//            return null;

//        return FileUtils.GetFileName(data.ReadStruct<UnicodeString>().Read());
//    }
//}
