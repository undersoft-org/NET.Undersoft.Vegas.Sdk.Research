using System.IO;
using System.Linq;

namespace System.Dealer
{
    public interface IFigureFormatter
    {
        int SerialCount { get; set; }
        int DeserialCount { get; set; }
        int ProgressCount { get; set; }
        int ItemsCount { get; }

        int Serialize(Stream tostream, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary);
        int Serialize(IFigurePacket buffor, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary);

        object Deserialize(Stream fromstream, bool driveon = true, FigureFormat serialFormat = FigureFormat.Binary);
        object Deserialize(ref object fromarray, bool driveon = true, FigureFormat serialFormat = FigureFormat.Binary);

        object[] GetMessage();
        object GetHeader();
    }

    public interface IDealSource
    {
        object Emulate(object source, string name = null);
        object Impact(object source, string name = null);
        object Locate(string path = null);
    }

   
}