
namespace System.Instants
{
    public interface IFigurePacket
    {
        ServiceSite Site { get; }

        long SerialPacketSize { get; set; }
        int  SerialPacketOffset { get; set; }

        byte[] SerialPacket { get; set; }
        IntPtr SerialPacketPtr { get; }

        int SerialPacketId { get; set; }

        long DeserialPacketSize { get; set; }
        int  DeserialPacketOffset { get; set; }

        byte[] DeserialPacket { get; }
        IntPtr DeserialPacketPtr { get; }
      
        int DeserialPacketId { get; set; }
    }
}



