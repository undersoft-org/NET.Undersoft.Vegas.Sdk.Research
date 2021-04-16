using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Extract;
using Microsoft.Win32.SafeHandles;

namespace System.Extract.Stock
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    public class UnsafeNative
    {
        public UnsafeNative() { }       

#if !NET40Plus

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, ExactSpelling = false)]
        [SecurityCritical]
        internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr va_list_arguments);

        [SecurityCritical]
        internal static string GetMessage(int errorCode)
        {
            StringBuilder stringBuilder = new StringBuilder(512);
            if (UnsafeNative.FormatMessage(12800, IntPtr.Zero, errorCode, 0, stringBuilder, stringBuilder.Capacity, IntPtr.Zero) != 0)
            {
                return stringBuilder.ToString();
            }
            return string.Concat("UnknownError_Num ", errorCode);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEM_INFO
        {
            internal _PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort dwProcessorLevel;
            public ushort dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }

        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            FileMapExecute = 0x0020,
        }

        [Flags]
        internal enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000,
        }
        /// <summary>
        /// Cannot create a file when that file already exists.
        /// </summary>
        internal const int ERROR_ALREADY_EXISTS = 0xB7; // 183
        /// <summary>
        /// The system cannot open the file.
        /// </summary>
        internal const int ERROR_TOO_MANY_OPEN_FILES = 0x4; // 4
        /// <summary>
        /// Access is denied.
        /// </summary>
        internal const int ERROR_ACCESS_DENIED = 0x5; // 5
        /// <summary>
        /// The system cannot find the file specified.
        /// </summary>
        internal const int ERROR_FILE_NOT_FOUND = 0x2; // 2

        [DllImport("kernel32.dll", CharSet = CharSet.None, SetLastError = true)]
        [SecurityCritical]
        internal static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
        [SecurityCritical]
        internal static extern SafeMemoryMappedFileHandle CreateFileMapping(SafeFileHandle hFile, IntPtr lpAttributes, FileMapProtection fProtect, int dwMaxSizeHi, int dwMaxSizeLo, string lpName);
        internal static SafeMemoryMappedFileHandle CreateFileMapping(SafeFileHandle hFile, FileMapProtection flProtect, Int64 ddMaxSize, string lpName)
        {
            int hi = (Int32)(ddMaxSize / Int32.MaxValue);
            int lo = (Int32)(ddMaxSize % Int32.MaxValue);
            return CreateFileMapping(hFile, IntPtr.Zero, flProtect, hi, lo, lpName);
        }

        [DllImport("kernel32.dll")]
        internal static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO lpSystemInfo);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeMemoryMappedViewHandle MapViewOfFile(
            SafeMemoryMappedFileHandle hFileMappingObject,
            FileMapAccess dwDesiredAccess,
            UInt32 dwFileOffsetHigh,
            UInt32 dwFileOffsetLow,
            UIntPtr dwNumberOfBytesToMap);
        internal static SafeMemoryMappedViewHandle MapViewOfFile(SafeMemoryMappedFileHandle hFileMappingObject, FileMapAccess dwDesiredAccess, ulong ddFileOffset, UIntPtr dwNumberofBytesToMap)
        {
            uint hi = (UInt32)(ddFileOffset / UInt32.MaxValue);
            uint lo = (UInt32)(ddFileOffset % UInt32.MaxValue);
            return MapViewOfFile(hFileMappingObject, dwDesiredAccess, hi, lo, dwNumberofBytesToMap);
        }

        [DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true, ThrowOnUnmappableChar = true)]
        internal static extern SafeMemoryMappedFileHandle OpenFileMapping(
             uint dwDesiredAccess,
             bool bInheritHandle,
             string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

#endif
    }
}
