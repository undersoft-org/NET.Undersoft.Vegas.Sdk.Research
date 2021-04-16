using System;
using System.Extract;
using System.Runtime.InteropServices;
using System.IO;

namespace System.Extract.Stock
{
    public unsafe sealed class StockContext : IStockContext, IDisposable
    {     
        private MemoryStream msReceive = new MemoryStream();
        private MemoryStream msRead = new MemoryStream();
        private byte[] binReceive = new byte[0];
        private byte[] binSend = new byte[0];
        public IntPtr binSendPtr;
        public IntPtr binReceivePtr;

        public StockContext()
        {
        }

        public string Place
        { get; set; }
        public string File
        { get; set; }

        public ushort StockId
        { get; set; } = 0;
        public ushort SectorId
        { get; set; } = 0;

        public int NodeCount { get; set; } = 50;
        public int ServerCount { get; set; } = 1;
        public int ClientCount { get; set; } = 1;
        public int ClientWaitCount = 0;
        public int ServerWaitCount = 0;
        public int Elements { get; set; } = 1;
        public int WriteCount = 0;
        public int ReadCount = 0;

        public long BufferSize
        {
            get; set;
        } = 1048576;
      
        public long UsedSize
        {
            get; set;
        } = 0;
        public long FreeSize
        {
            get; set;
        } = 0;

        public int ItemSize
        { get; set; } = -1;
        public int ItemCount
        { get; set; } = -1;
        public long ItemCapacity
        { get; set; } = -1;

        public int ObjectPosition
        {
            get; set;
        } = 0;
        public int ObjectsLeft
        {
            get; set;
        } = 0;

        public byte[] SerialPacket
        {
            get
            {
                return binSend;
            }
            set
            {
                binSend = value;
                if (binSend != null && SerialPacketOffset > 0) 
                {
                    long size = binSend.Length - SerialPacketOffset;
                    new byte[] { (byte) 'V',
                                 (byte) 'S',
                                 (byte) 'S',
                                 (byte) 'P' }.CopyTo(binSend, 0);
                    size.GetBytes().CopyTo(binSend, 4);
                    ObjectPosition.GetBytes().CopyTo(binSend, 12);
                    GCHandle gc = GCHandle.Alloc(binSend, GCHandleType.Pinned);
                    binSendPtr = GCHandle.ToIntPtr(gc);
                }
            }
        }
        public IntPtr SerialPacketPtr
        {
            get { return GCHandle.FromIntPtr(binSendPtr).AddrOfPinnedObject();  }
        }
        public int SerialPacketId
        {
            get; set;
        } = 0;

        public long SerialPacketSize
        { get; set; } = 0;
        public int  SerialPacketOffset
        {
            get; set;
        } = 16;

        public byte[] DeserialPacket
        {
            get
            {
                byte[] result = null;
                lock (binReceive)
                {
                    SerialPacketSize = 0;
                    result = binReceive;
                    binReceive = new byte[0];
                }
                return result;
            }
        }
        public IntPtr DeserialPacketPtr
        {
            get { return GCHandle.FromIntPtr(binReceivePtr).AddrOfPinnedObject() + SerialPacketOffset; }
        }
        public int DeserialPacketId
        {
            get; set;
        } = 0;

        public long DeserialPacketSize
        { get; set; } = 0;
        public int  DeserialPacketOffset
        {
            get; set;
        } = 16;

        public void ReceiveBytes(IntPtr buffer, long received)
        {
            lock (binReceive)
            {
                SerialPacketSize = *((int*)(buffer + 4));
                DeserialPacketId = *((int*)(buffer + 12));
            }
        }
        public MarkupType ReceiveBytes(byte[] buffer, long received)
        {

            MarkupType noiseKind = MarkupType.None;
            lock (binReceive)
            {
                int offset = 0, length = (int)received;
                bool inprogress = false;
                if (SerialPacketSize == 0)
                {

                    SerialPacketSize = BitConverter.ToInt64(buffer, 4);
                    DeserialPacketId = BitConverter.ToInt32(buffer, 12);
                    binReceive = new byte[SerialPacketSize];
                    GCHandle gc = GCHandle.Alloc(binReceive, GCHandleType.Pinned);
                    binReceivePtr = GCHandle.ToIntPtr(gc);
                    offset = SerialPacketOffset;
                    length -= SerialPacketOffset;
                }

                if (SerialPacketSize > 0)
                    inprogress = true;

                SerialPacketSize -= length;

                if (SerialPacketSize < 1)
                {
                    long endPosition = received;
                    noiseKind = buffer.SeekMarkup(out endPosition, SeekDirection.Backward);
                }

                int destid = (binReceive.Length - ((int)SerialPacketSize + length));
                if (inprogress)
                {
                    fixed (byte* msgbuff = buffer)
                    {
                        Extractor.CopyBlock(GCHandle.FromIntPtr(binReceivePtr).AddrOfPinnedObject().ToPointer(), (ulong)destid, msgbuff, (ulong)offset, (ulong)length);
                      //  Extractor.CopyBlock(GCHandle.FromIntPtr(binReceivePtr).AddrOfPinnedObject() + destid, new IntPtr(msgbuff) + offset, (ulong)length);
                    }
                }
            }
            return noiseKind;
        }
        public MarkupType ReceiveBytes(byte[] buffer, int received)
        {
            MarkupType noiseKind = MarkupType.None;
            lock (binReceive)
            {
                int offset = 0, length = received;
                bool inprogress = false;

                if (SerialPacketSize == 0)
                {
                    SerialPacketSize = BitConverter.ToInt64(buffer, 4);
                    DeserialPacketId = BitConverter.ToInt32(buffer, 12);
                    binReceive = new byte[SerialPacketSize];
                    GCHandle gc = GCHandle.Alloc(binReceive, GCHandleType.Pinned);
                    binReceivePtr = GCHandle.ToIntPtr(gc);
                    offset = SerialPacketOffset;
                    length -= SerialPacketOffset;

                }
                if (SerialPacketSize > 0)
                    inprogress = true;

                SerialPacketSize -= length;

                if (SerialPacketSize < 1)
                {
                    long endPosition = received;
                    noiseKind = buffer.SeekMarkup(out endPosition, SeekDirection.Backward);
                }
                int destid = (binReceive.Length - ((int)SerialPacketSize + length));
                if (inprogress)
                {
                    fixed (void* msgbuff = buffer)
                    {
                        Extractor.CopyBlock(GCHandle.FromIntPtr(binReceivePtr).AddrOfPinnedObject().ToPointer(), (ulong)destid, msgbuff, (ulong)offset, (ulong)length);
                    }
                }
            }
            return noiseKind;
        }

        public void WriteStock(IStock drive)
        {
            if (drive != null)
            {
                GCHandle handler = GCHandle.Alloc(SerialPacket, GCHandleType.Pinned);
                IntPtr rawpointer = handler.AddrOfPinnedObject();
                drive.BufferSize = SerialPacket.Length;
                drive.WriteHeader();
                drive.Write(rawpointer, SerialPacket.Length);
                handler.Free();
            }
        }
        public void WriteStockPtr(IStock drive)
        {
            if (drive != null)
            {
                drive.BufferSize = SerialPacketSize;
                drive.WriteHeader();
                drive.Write(SerialPacketPtr, SerialPacketSize);
            }
        }

        public object ReadStock(IStock drive)
        {
            if (drive != null)
            {
                drive.ReadHeader();
                BufferSize = drive.BufferSize;
                byte[] bufferread = new byte[BufferSize];
                GCHandle handler = GCHandle.Alloc(bufferread, GCHandleType.Pinned);
                IntPtr rawpointer = handler.AddrOfPinnedObject();
                drive.Read(rawpointer, BufferSize, 0L);
                ReceiveBytes(bufferread, BufferSize);
                handler.Free();
            }
            return DeserialPacket;
        }
        public IntPtr ReadStockPtr(IStock drive)
        {
            if (drive != null)
            {
                drive.ReadHeader();
                BufferSize = drive.BufferSize;
                binReceive = new byte[BufferSize];
                GCHandle handler = GCHandle.Alloc(binReceive, GCHandleType.Pinned);
                binReceivePtr = GCHandle.ToIntPtr(handler);
                IntPtr rawpointer = handler.AddrOfPinnedObject();
                drive.Read(rawpointer, BufferSize, 0);
                ReceiveBytes(rawpointer, BufferSize);
            }
            return DeserialPacketPtr;
        }

        public void Dispose()
        {
            msRead.Dispose();
            msReceive.Dispose();
            if (!binReceivePtr.Equals(IntPtr.Zero))
            {
                GCHandle gc = GCHandle.FromIntPtr(binReceivePtr);
                gc.Free();
            }
            if (!binSendPtr.Equals(IntPtr.Zero))
            {
                GCHandle gc = GCHandle.FromIntPtr(binSendPtr);
                gc.Free();
            }
            binReceive = null;
            binSend = null;
        }
    }
}
