using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Instants;
using System.Reflection;


namespace System.Dealer
{
    [Serializable]
    public class DealMessage : IFigureFormatter, IDisposable
    {
        [NonSerialized]
        private DealTransfer transaction;

        private DirectionType direction;    

        public DealMessage()
        {
            content = new object();
            SerialCount = 0;
            DeserialCount = 0;
            direction = DirectionType.Receive;
        }
        public DealMessage(DealTransfer _transaction, DirectionType _direction, object message = null)
        {
            transaction = _transaction;
            direction = _direction;

            if (message != null)
                Content = message;
            else
                content = new object();

            SerialCount = 0;
            DeserialCount = 0;
        }

        private object content;
        public object Content
        {
            get { return content; }
            set { transaction.Manager.MessageContent(ref content, value, direction); }
        }

        public string Notice
        { get; set; }

        public void Dispose()
        {
            content = null;
        }

        #region Serialization

        public int Serialize(Stream tostream, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            if (serialFormat == FigureFormat.Binary)
                return this.SetRaw(tostream);
            else if (serialFormat == FigureFormat.Json)
                return this.SetJson(tostream);
            else
                return -1;
        }
        public int Serialize(IFigurePacket buffor, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            if (serialFormat == FigureFormat.Binary)
                return this.SetRaw(buffor);
            else if (serialFormat == FigureFormat.Json)
                return this.SetJson(buffor);
            else
                return -1;
        }

        public object Deserialize(Stream fromstream, FigureFormat serialFormat = FigureFormat.Binary)
        {
            if (serialFormat == FigureFormat.Binary)
                return this.GetRaw(fromstream);
            else if (serialFormat == FigureFormat.Json)
                return this.GetJson(fromstream);
            else
                return -1;
        }
        public object Deserialize(ref object fromarray, FigureFormat serialFormat = FigureFormat.Binary)
        {
            if (serialFormat == FigureFormat.Binary)
                return this.GetRaw(ref fromarray);
            else if (serialFormat == FigureFormat.Json)
                return this.GetJson(ref fromarray);
            else
                return -1;
        }

        public object[] GetMessage()
        {
            if(content != null)
                return (IFigureFormatter[])content;
            return null;
        }
        public object GetHeader()
        {
            if (direction == DirectionType.Send)
                return transaction.MyHeader.Content;
            else
                return transaction.HeaderReceived.Content;
        }

        public int SerialCount { get; set; }
        public int DeserialCount { get; set; }
        public int ProgressCount { get; set; }
        public int ItemsCount { get { return (content != null) ? ((IFigureFormatter[])content).Sum(t => t.ItemsCount): 0; } }
        public int ObjectsCount { get { return (content != null) ? ((IFigureFormatter[])content).Length : 0;  } }

        #endregion
    }

    public static class RawMessage
    {
        public static int SetRaw(this DealMessage bank, Stream tostream)
        {
            if (tostream == null) tostream = new MemoryStream();
            BinaryFormatter binform = new BinaryFormatter();
            binform.Serialize(tostream, bank);
            return (int)tostream.Length;
        }
        public static int SetRaw(this DealMessage bank, IFigurePacket tostream)
        {
            int offset = tostream.SerialPacketOffset;
            MemoryStream ms = new MemoryStream();
            ms.Write(new byte[offset], 0, offset);
            BinaryFormatter binform = new BinaryFormatter();
            binform.Serialize(ms, bank);
            tostream.SerialPacket = ms.ToArray();
            ms.Dispose();
            return tostream.SerialPacket.Length;
        }
        public static DealMessage GetRaw(this DealMessage bank, Stream fromstream)
        {
            try
            {
                BinaryFormatter binform = new BinaryFormatter();
                return (DealMessage)binform.Deserialize(fromstream);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DealMessage GetRaw(this DealMessage bank, ref object fromarray)
        {
            try
            {
                MemoryStream ms = new MemoryStream((byte[])fromarray);
                BinaryFormatter binform = new BinaryFormatter();
                DealMessage _bank = (DealMessage)binform.Deserialize(ms);
                ms.Dispose();
                return _bank;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }

    public static class JsonMessage
    {
        public static int SetJson(this DealMessage tmsg, StringBuilder stringbuilder)
        {
            stringbuilder.AppendLine(tmsg.SetJsonString());
            return stringbuilder.Length;
        }
        public static int SetJson(this DealMessage tmsg, Stream tostream)
        {
            BinaryWriter binwriter = new BinaryWriter(tostream);
            binwriter.Write(tmsg.SetJsonString());
            return (int)tostream.Length;
        }
        public static int SetJson(this DealMessage tmsg, IFigurePacket buffor)
        {
            buffor.SerialPacket = Encoding.UTF8.GetBytes(tmsg.SetJsonString());
            return buffor.SerialPacket.Length;
        }

        public static string SetJsonString(this DealMessage tmsg)
        {
            IDictionary<string, object> toJson = tmsg.SetJsonBag();
            return JsonParser.ToJson(toJson);
        }
        public static string SetJsonString(this DealMessage tmsg, IDictionary<string, object> jsonbag)
        {
            return JsonParser.ToJson(jsonbag);
        }

        public static Dictionary<string, object> SetJsonBag(this DealMessage tmsg)
        {
            return new Dictionary<string, object>() { { "DealMessage", JsonParserProperties.GetJsonProperties(typeof(DealMessage))
                                                                       .Select(k => new KeyValuePair<string, object>(k.Name, k.GetValue(tmsg, null)))
                                                                       .ToDictionary(k => k.Key, v => v.Value) } };
        }

        public static Dictionary<string, DealMessage> GetJsonObject(this DealMessage tmsg, IDictionary<string, object> _bag)
        {
            Dictionary<string, DealMessage> result = new Dictionary<string, DealMessage>();
            IDictionary<string, object> bags = _bag;
            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(out inst, typeof(DealMessage));
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                DealMessage deck = (DealMessage)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }
        public static Dictionary<string, DealMessage> GetJsonObject(this DealMessage tmsg, string JsonString)
        {
            Dictionary<string, DealMessage> result = new Dictionary<string, DealMessage>();
            Dictionary<string, object> bags = new Dictionary<string, object>();
            tmsg.GetJsonBag(JsonString, bags);

            foreach (KeyValuePair<string, object> bag in bags)
            {
                object inst = new object();
                IEnumerable<PropertyInfo> map = JsonParser.PrepareInstance(out inst, typeof(DealMessage));
                JsonParser.DeserializeType(map, (IDictionary<string, object>)bag.Value, inst);
                DealMessage deck = (DealMessage)inst;
                result.Add(bag.Key, deck);
            }
            return result;
        }

        public static void GetJsonBag(this DealMessage tmsg, string JsonString, IDictionary<string, object> _bag)
        {
            _bag.AddRange(JsonParser.FromJson(JsonString));
        }

        public static DealMessage GetJson(this DealMessage tmsg, string jsonstring)
        {
            try
            {
                DealMessage trs = tmsg.GetJsonObject(jsonstring)["DealMessage"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DealMessage GetJson(this DealMessage tmsg, StringBuilder stringbuilder)
        {
            try
            {
                StringBuilder sb = stringbuilder;
                DealMessage trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DealMessage GetJson(this DealMessage tmsg, Stream fromstream)
        {
            try
            {
                fromstream.Position = 0;
                byte[] array = new byte[4096];
                int read = 0;
                StringBuilder sb = new StringBuilder();
                while ((read = fromstream.Read(array, 0, array.Length)) > 0)
                {
                    sb.Append(array.Cast<char>());
                }
                DealMessage trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];
                sb = null;
                fromstream.Dispose();
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static DealMessage GetJson(this DealMessage tmsg, ref object fromarray)
        {
            try
            {
                DealMessage trs = null;
                if (fromarray is String)
                {
                    trs = tmsg.GetJsonObject((String)fromarray)["DealMessage"];
                }
                else
                {
                    byte[] _fromarray = (byte[])fromarray;
                    StringBuilder sb = new StringBuilder();

                    sb.Append(_fromarray.ToChars(CharEncoding.UTF8));
                    trs = tmsg.GetJsonObject(sb.ToString())["DealMessage"];

                    fromarray = null;
                    _fromarray = null;
                    sb = null;
                }
                return trs;
            }
            catch (Exception ex)
            {
                return null;
            }           
        }
    }
}
