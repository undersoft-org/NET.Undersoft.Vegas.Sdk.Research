using System;
using System.IO;
using System.Collections;
using System.Multemic;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;
using System.Extract;

namespace System.Dealer
{
    public sealed class TransferContext : ITransferContext, IDisposable
    {
        #region Private / NonSerialized
        private const int Buffer_Size = 4096;
        private int Packet_Offset = 16;

        [NonSerialized] private bool disposed = false;

        [NonSerialized] private byte[] headerbuffer = new byte[Buffer_Size];
        [NonSerialized] private byte[] messagebuffer = new byte[Buffer_Size];
        [NonSerialized] public IntPtr headerBufferHandler;
        [NonSerialized] public IntPtr messageBufferHandler;
        [NonSerialized] public IntPtr headerBufferAddress;
        [NonSerialized] public IntPtr messageBufferAddress;

        [NonSerialized] private Socket _listener;
        [NonSerialized] private readonly DealStream instream;
        [NonSerialized] private readonly DealStream outstream;
        private int id;
        [NonSerialized] private StringBuilder sb = new StringBuilder();
        [NonSerialized] public MemoryStream msReceive;
        [NonSerialized] public MemoryStream msSend;

        [NonSerialized] public byte[] binReceive = new byte[0];
        [NonSerialized] public byte[] binSend = new byte[0];

        [NonSerialized] public IntPtr binReceiveHandler;
        [NonSerialized] public IntPtr binSendHandler;
        [NonSerialized] public IntPtr binReceiveAddress;
        [NonSerialized] public IntPtr binSendAddress;

        [NonSerialized] public StringBuilder requestBuilder = new StringBuilder();
        [NonSerialized] public StringBuilder responseBuilder = new StringBuilder();

        [NonSerialized] public Hashtable httpHeaders = new Hashtable();
        [NonSerialized] public Hashtable httpOptions = new Hashtable();

        [NonSerialized] private DealTransfer tr;

        [NonSerialized] private IDeck<byte[]> resources;

        public ManualResetEvent HeaderSentNotice { get; set; } = new ManualResetEvent(false);
        public ManualResetEvent MessageSentNotice { get; set; } = new ManualResetEvent(false);
        public ManualResetEvent HeaderReceivedNotice { get; set; } = new ManualResetEvent(false);
        public ManualResetEvent MessageReceivedNotice { get; set; } = new ManualResetEvent(false);
        public ManualResetEvent BatchesReceivedNotice { get; set; } = new ManualResetEvent(false);
        #endregion
     
        public IDeck<byte[]> Resources { get { return resources; } set { resources = value; } }
        public DealProtocol Protocol { get; set; } = DealProtocol.NONE;
        public ProtocolMethod Method { get; set; } = ProtocolMethod.NONE;
       
        public TransferContext(Socket listener, int _id = -1, bool withStream = false)
        {
            this._listener = listener;
            if (withStream)
            {
                this.instream = new DealStream(listener);
                this.outstream = new DealStream(listener);
            }

            GCHandle gc = GCHandle.Alloc(messagebuffer, GCHandleType.Pinned);
            messageBufferHandler = GCHandle.ToIntPtr(gc);
            messageBufferAddress = gc.AddrOfPinnedObject();
            gc = GCHandle.Alloc(headerbuffer, GCHandleType.Pinned);
            headerBufferHandler = GCHandle.ToIntPtr(gc);
            headerBufferAddress = gc.AddrOfPinnedObject();

            this.id = _id;
            this.Close = false;
            this.Denied = false;
            this.ObjectPosition = 0;
            this.ObjectsLeft = 0;
            this.DeserialPacketId = 0;
            this.SerialPacketSize = 0;
            this.SendMessage = true;
            this.ReceiveMessage = true;
            this.disposed = true;

            HeaderSentNotice.Reset();
            HeaderReceivedNotice.Reset();
            MessageSentNotice.Reset();
            MessageReceivedNotice.Reset();
            BatchesReceivedNotice.Reset();
        }

        public int Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public bool Close
        { get; set; }
        public bool Denied
        { get; set; }
       
        public bool Synchronic
        { get; set; }
        public bool SendMessage
        { get; set; }
        public bool ReceiveMessage
        { get; set; }

        public int BufferSize
        {
            get
            {
                return Buffer_Size;
            }
        }

        public byte[] HeaderBuffer
        {
            get
            {
                return this.headerbuffer;
            }
        }
        public byte[] MessageBuffer
        {
            get
            {
                return this.messagebuffer;
            }
        }

        public long SerialPacketSize
        { get; set; }
        public int SerialPacketOffset
        {
            get
            {
                return Packet_Offset;
            }
            set
            {
                Packet_Offset = value;
            }
        }

        public byte[] SerialPacket
        {
            get
            {
                return binSend;
            }
            set
            {
                if (value != null)
                {
                    lock (binSend)
                    {
                        disposed = false;
                        binSend = value;
                        if (Protocol != DealProtocol.HTTP)
                        {
                            long size = binSend.Length - SerialPacketOffset;
                            new byte[] { (byte) 'D',
                                     (byte) 'V',
                                     (byte) 'S',
                                     (byte) 'P' }.CopyTo(binSend, 0);
                            BitConverter.GetBytes(size).CopyTo(binSend, 4);
                            BitConverter.GetBytes(ObjectPosition).CopyTo(binSend, 12);
                        }
                        value = null;
                    }
                }
            }
        }
        public int SerialPacketId
        {
            get; set;
        }

        public IntPtr SerialPacketPtr => binSendAddress;

        public IntPtr DeserialPacketPtr => binReceiveAddress;

        public byte[] DeserialPacket
        {
            get
            {
                byte[] result = null;
                lock (binReceive)
                {
                    disposed = false;
                    SerialPacketSize = 0;
                    result = binReceive;
                    binReceive = new byte[0];
                }
                return result;
            }
        }
        public int DeserialPacketId
        { get; set; }

        public long DeserialPacketSize
        { get; set; }
        public int DeserialPacketOffset
        {
            get
            {
                return Packet_Offset;
            }
            set
            {
                Packet_Offset = value;
            }
        }

        public MarkupType IncomingHeader(int received)
        {
            disposed = false;
            MarkupType noiseKind = MarkupType.None;
            if (Protocol == DealProtocol.NONE)
                IdentifyProtocol();
            if (Protocol == DealProtocol.DOTP)
                return SyncHeader(received);
            else if (Protocol == DealProtocol.HTTP)
                return HttpHeader(received);
            return noiseKind;
        }
        public unsafe MarkupType SyncHeader(int received)
        {
            MarkupType noiseKind = MarkupType.None;

            lock (binReceive)
            {
                int offset = 0, length = received;
                bool inprogress = false;
                if (SerialPacketSize == 0)
                {
                    SerialPacketSize = *((int*)(headerBufferAddress + 4).ToPointer());
                    DeserialPacketId = *((int*)(headerBufferAddress + 12).ToPointer());

                    binReceive = new byte[SerialPacketSize];
                    GCHandle gc = GCHandle.Alloc(binReceive, GCHandleType.Pinned);
                    binReceiveHandler = GCHandle.ToIntPtr(gc);
                    binReceiveAddress = gc.AddrOfPinnedObject();

                    offset = SerialPacketOffset;
                    length -= SerialPacketOffset;
                }

                if (SerialPacketSize > 0)
                    inprogress = true;

                SerialPacketSize -= length;

                if (SerialPacketSize < 1)
                {
                    long endPosition = length;
                    noiseKind = HeaderBuffer.SeekMarkup(out endPosition, SeekDirection.Backward);
                }

                int destid = (int)(binReceive.Length - (SerialPacketSize + length));

                if (inprogress)
                {
                    Extractor.CopyBlock(binReceiveAddress, destid, headerBufferAddress, offset, length);
                }
            }
            return noiseKind;
        }
        public MarkupType HttpHeader(int received)
        {
            MarkupType noiseKind = MarkupType.None;

            lock (binReceive)
            {
                if (SerialPacketSize == 0)
                {
                    SerialPacketSize = 1;
                    msReceive = new MemoryStream();
                }

                msReceive.Write(HeaderBuffer, 0, received);

                if (received < BufferSize)
                {
                    SerialPacketSize = 0;
                    msReceive.Position = 0;
                    ParseRequest(msReceive);
                    ReadHeaders(msReceive);
                    if (VerifyRequest())
                        binReceive = msReceive.ToArray().Skip(Convert.ToInt32(msReceive.Position)).ToArray();
                    else
                        Denied = true;                    
                }
            }
            return noiseKind;
        }

        public unsafe MarkupType IncomingMessage(int received)
        {
            disposed = false;
            MarkupType noiseKind = MarkupType.None;
            if (Protocol == DealProtocol.DOTP)
                return SyncMessage(received);
            return noiseKind;
        }
        public unsafe MarkupType SyncMessage(int received)
        {
            MarkupType noiseKind = MarkupType.None;

            lock (binReceive)
            {
                int offset = 0, length = received;
                bool inprogress = false;

                if (SerialPacketSize == 0)
                {
                    SerialPacketSize = *((int*)(messageBufferAddress + 4).ToPointer());
                    DeserialPacketId = *((int*)(messageBufferAddress + 12).ToPointer());

                    binReceive = new byte[SerialPacketSize];
                    GCHandle gc = GCHandle.Alloc(binReceive, GCHandleType.Pinned);
                    binReceiveHandler = GCHandle.ToIntPtr(gc);
                    binReceiveAddress = gc.AddrOfPinnedObject();

                    offset = SerialPacketOffset;
                    length -= SerialPacketOffset;
                }

                if (SerialPacketSize > 0)
                    inprogress = true;

                SerialPacketSize -= length;

                if (SerialPacketSize < 1)
                {
                    long endPosition = length;
                    noiseKind = MessageBuffer.SeekMarkup(out endPosition, SeekDirection.Backward);
                }

                int destid = (int)(binReceive.Length - (SerialPacketSize + length));
                if (inprogress)
                {
                    Extractor.CopyBlock(binReceiveAddress, destid, messageBufferAddress, offset, length);
                }
            }
            return noiseKind;
        }

        public int ObjectPosition
        { get; set; }
        public int ObjectsLeft
        { get; set; }

        public Socket Listener
        {
            get
            {
                return this._listener;
            }
            set
            {
                this._listener = value;
            }
        }

        public DealTransfer Transfer
        {
            get
            {
                return this.tr;
            }
            set
            {
                if (value.Context == null)
                {
                    value.Context = this;
                    if (value.Identity != null)
                        value.MyHeader.BindContext(value.Context);
                }
                if (value.MyMessage.Content != null)
                {
                    if (value.MyMessage.Content.GetType() == typeof(object[][]))
                        value.MyHeader.Context.ObjectsCount = ((object[][])value.MyMessage.Content).Length;
                }
                this.tr = value;
            }
        }

        public ServiceSite Site
        {
            get
            {
                return Transfer.MyHeader.Context.IdentitySite;
            }
        }

        public DealProtocol IdentifyProtocol()
        {
            StringBuilder sb = new StringBuilder();
            Protocol = DealProtocol.NONE;
            ProtocolMethod method = ProtocolMethod.NONE;
            for (int i = 0; i < HeaderBuffer.Length; i++)
            {
                MarkupType splitter = MarkupType.None;
                HeaderBuffer[i].IsSpliter(out splitter);
                if ((splitter != MarkupType.Empty) &&
                    (splitter != MarkupType.Space) &&
                    (splitter != MarkupType.Line))
                    sb.Append(HeaderBuffer[i].ToChars(CharEncoding.UTF8));
                if (sb.Length > 3)
                {
                    method = ProtocolMethod.NONE;
                    Enum.TryParse(sb.ToString().ToUpper(), out method);
                    if (method != ProtocolMethod.NONE)
                    {
                        switch (method)
                        {
                            case ProtocolMethod.DEAL:
                                Protocol = DealProtocol.DOTP;
                                break;
                            case ProtocolMethod.SYNC:
                                Protocol = DealProtocol.HTTP;
                                break;
                            case ProtocolMethod.GET:
                                Protocol = DealProtocol.HTTP;
                                break;
                            case ProtocolMethod.POST:
                                Protocol = DealProtocol.HTTP;
                                break;
                            case ProtocolMethod.OPTIONS:
                                Protocol = DealProtocol.HTTP;
                                break;
                            default:
                                Protocol = DealProtocol.NONE;
                                break;
                        }
                    }
                    Method = method;
                    if (Protocol != DealProtocol.NONE)
                    {
                        sb = null;
                        return Protocol;
                    }
                }
            }
            sb = null;
            return Protocol;
        }

        DealEvent SendEcho
        { get; set; }
        public string Echo
        {
            get
            {
                return this.sb.ToString();
            }
        }

        public void Append(string text)
        {
            this.sb.Append(text);
        }

        public void Reset()
        {
            if (!disposed)
            {
                sb.Clear();
                sb = new StringBuilder();
                msSend.Dispose();
                msSend = new MemoryStream();
                msReceive.Dispose();
                msReceive = new MemoryStream();

                lock (binReceive)
                {
                    if (!binReceiveHandler.Equals(IntPtr.Zero))
                    {
                        GCHandle gc = GCHandle.FromIntPtr(binReceiveHandler);
                        gc.Free();
                        binReceive = new byte[0];
                    }
                }
                lock (binSend)
                    binSend = new byte[0];
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                sb.Clear();
                msSend.Dispose();
                msReceive.Dispose();
                GCHandle gc;
                lock (binReceive)
                {
                    if (!binReceiveHandler.Equals(IntPtr.Zero))
                    {
                        gc = GCHandle.FromIntPtr(binReceiveHandler);
                        gc.Free();                       
                    }
                    binReceive = null;
                }
                lock (binSend)
                {
                    if (!binSendHandler.Equals(IntPtr.Zero))
                    {
                        gc = GCHandle.FromIntPtr(binSendHandler);
                        gc.Free();
                    }
                    binSend = null;
                }

                gc = GCHandle.FromIntPtr(messageBufferHandler);
                gc.Free();
                messagebuffer = null;
                gc = GCHandle.FromIntPtr(headerBufferHandler);
                gc.Free();
                headerbuffer = null;

                HeaderSentNotice.Dispose();
                HeaderReceivedNotice.Dispose();
                MessageSentNotice.Dispose();
                MessageReceivedNotice.Dispose();
                BatchesReceivedNotice.Dispose();

                disposed = true;
            }
        }

        #region Http Raw Stream Processing - TODO - After testing create HttpContext and HttpProcessor class, then move below there  

        public StringBuilder RequestBuilder { get { return requestBuilder; } set { requestBuilder = value; } }
        public StringBuilder ResponseBuilder { get { return responseBuilder; } set { responseBuilder = value; } }

        [NonSerialized] private String http_method;
        [NonSerialized] private String http_url;
        [NonSerialized] private String http_protocol_version;

        public Hashtable HttpHeaders { get { return httpHeaders; } set { httpHeaders = value; } }
        public Hashtable HttpOptions { get { return httpOptions; } set { httpOptions = value; } }


        public void ParseRequest(Stream ms)
        {
            String request = streamReadLine(ms);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_version = tokens[2];

        }

        public void ReadHeaders(Stream ms)
        {
            String line;
            while ((line = streamReadLine(ms)) != null)
            {
                if (line.Equals(""))
                    return;

                int separator = line.IndexOf(':');
                if (separator == -1)
                    throw new Exception("invalid http header line: " + line);

                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                    pos++; // strip any spaces                

                string value = line.Substring(pos, line.Length - pos);
                HttpHeaders[name] = value;
            }
        }

        public bool VerifyRequest()
        {
            bool verified = false;
            string ip = tr.MyHeader.Context.RemoteEndPoint.Address.ToString();

            if (HttpHeaders.ContainsKey("DealerToken"))
                if (HttpHeaders["DealerToken"].ToString() != "")
                {
                    string token = HttpHeaders["DealerToken"].ToString();
                    MemberIdentity di = null;
                    if (DealServer.Security.Register(token, out di, ip))
                    {
                        verified = true;
                        HttpOptions["DealerToken"] = di.Token;
                        HttpOptions["DealerUserId"] = di.UserId;
                        HttpOptions["DealerDeptId"] = di.DeptId;
                    }
                }

            if (!verified)
                if (HttpHeaders.ContainsKey("Authorization"))
                    if (HttpHeaders["Authorization"].ToString() != "")
                    {
                        string[] codes = HttpHeaders["Authorization"].ToString().Split(' ');
                        string decode64 = "";
                        string name = "";
                        string key = "";
                        if (codes.Length > 1)
                        {
                            decode64 = Encoding.UTF8.GetString(Convert.FromBase64String(codes[1]));
                            string[] namekey = decode64.Split(':');
                            name = namekey[0];
                            key = namekey[1];

                            MemberIdentity di = null;
                            if (DealServer.Security.Register(name, key, out di, ip))
                            {
                                verified = true;
                                HttpOptions["DealerToken"] = di.Token;
                                HttpOptions["DealerUserId"] = di.UserId;
                                HttpOptions["DealerDeptId"] = di.DeptId;
                            }
                        }
                    }

            return verified;
        }

        public void HandleGetRequest(string content_type = "text/html")
        {
            if (http_url.Equals("/") ||
                http_url.Equals(""))
                http_url = "/Index.html";

            if (!Resources.TryGet(http_url, out binSend))
            {
                MemoryStream ms = new MemoryStream();
                if (GetJavaScriptProject(ms))
                {
                    content_type = GetHttpExtensionType();
                    writeSuccess(content_type);
                }
                else
                {
                    if (File.Exists("../../Web" + http_url))
                    {
                        content_type = GetHttpExtensionType();
                        Stream fs = File.Open("../../Web" + http_url, FileMode.Open);
                        fs.CopyTo(ms);
                        fs.Close();
                        writeSuccess(content_type);
                    }
                    else
                        writeFailure();
                }

                if (HttpHeaders.ContainsKey("Connection"))
                    writeClose(HttpHeaders["Connection"].ToString());
                else
                    writeClose();

                if (content_type.Contains("text") ||
                    content_type.Contains("json"))
                {
                    ResponseBuilder.Append((ms.ToArray().ToChars(CharEncoding.UTF8)));
                    binSend = ResponseBuilder.ToString().ToBytes(CharEncoding.UTF8);
                    Resources.Add(http_url, binSend);
                }
                else
                {
                    binSend = ResponseBuilder.ToString().ToBytes(CharEncoding.UTF8).Concat(ms.ToArray()).ToArray();
                    Resources.Add(http_url, binSend);
                }
                ms.Dispose();
            }
        }
        public void HandlePostRequest(string content_type = "text/html")
        {
            writeSuccess(content_type);
            writeOptions();
            writeClose();
            string requestBuilder = RequestBuilder.ToString();//.Trim(new char[] { '\n', '\r' });
            ResponseBuilder.AppendLine(requestBuilder);
            string responseString = ResponseBuilder.ToString();//.TrimEnd(new char[] { '\n', '\r' });           
            binSend = responseString.ToBytes(CharEncoding.UTF8);
        }
        public void HandleOptionsRequest(string content_type = "text/html")
        {
            writeSuccess(content_type);
            writeOptions();
            writeClose();
            binSend = ResponseBuilder.ToString().ToBytes(CharEncoding.UTF8);
        }
        public void HandleDeniedRequest()
        {
            writeDenied();
            writeClose();
            binSend = ResponseBuilder.ToString().ToBytes(CharEncoding.UTF8);
        }

        public string GetHttpExtensionType()
        {
            string extension = http_url.Substring(http_url.LastIndexOf('.'));
            string result = null;
            httpExtensions.TryGetValue(extension, out result);
            return result = (result == null) ? "text/html" : result;
        }

        public bool   GetJavaScriptProject(MemoryStream ms)
        {
            string extension = http_url.Substring(http_url.LastIndexOf('.'));
            if (extension.Equals(".qjs") || extension.Equals(".bjs"))
            {
                string jspdir = "../../Web" + http_url.Substring(0, http_url.LastIndexOf('.'));
                List<string> jspfiles = new List<string>();
                DirSearch(jspdir, jspfiles);
                foreach (string jspfile in jspfiles)
                {
                    Stream fs = File.Open(jspfile, FileMode.Open);
                    fs.CopyTo(ms);
                    //ms.Write(("\n").ToBytes(CharEncoding.UTF8), 0, 1);
                    fs.Close();
                }
                return true;
            }
            return false;
        }

        public void DirSearch(string dir, List<string> jspfiles)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                    jspfiles.Add(f);
                foreach (string d in Directory.GetDirectories(dir))
                    DirSearch(d, jspfiles);
            }
            catch (Exception ex)
            {
            }
        }

        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            StringBuilder data = new StringBuilder();
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data.Append(((byte)next_char).ToChars(CharEncoding.UTF8));
            }
            return data.ToString();
        }

        private void writeOptions()
        {
            if(HttpOptions.Count > 0)
                foreach(DictionaryEntry option in HttpOptions)
                    ResponseBuilder.AppendLine(string.Format("{0}: {1}",option.Key, option.Value));

            ResponseBuilder.AppendLine("Accept: application/json");
            ResponseBuilder.AppendLine("Access-Control-Allow-Headers: content-type");
            ResponseBuilder.AppendLine("Access-Control-Allow-Origin: " + HttpHeaders["Origin"].ToString());
        }

        private void writeSuccess(string content_type = "text/html")
        {
            ResponseBuilder.AppendLine("HTTP/1.1 200 OK");
            ResponseBuilder.AppendLine("Content-Type: " + content_type);
        }

        private void writeFailure()
        {
            ResponseBuilder.AppendLine("HTTP/1.1 404 File not found");
        }

        private void writeDenied()
        {
            ResponseBuilder.AppendLine("HTTP/1.1 401.7 Access denied by authorization strategy on VSSP server");
        }

        private void writeClose(string state = "close")
        {
            ResponseBuilder.AppendLine("Connection: " + state);
            ResponseBuilder.AppendLine("");
        }

        [NonSerialized]
        private Dictionary<string, string> httpExtensions = new Dictionary<string, string>()
        {
            { ".html", "text/html" },
            { ".css",  "text/css"  },
            { ".less", "text/css"  },
            { ".png",  "image/png" },
            { ".ico",  "image/ico" },
            { ".jpg",  "image/jpg" },
            { ".bmp",  "image/bmp" },
            { ".gif",  "image/gif" },
            { ".js",   "text/javascript" },
            { ".qjs",  "text/javascript" },
            { ".bjs",  "text/babel" },
            { ".json", "application/json" },
            { ".woff", "font/woff" },
            { ".woff2","font/woff2" },
            { ".ttf",  "font/ttf" },
            { ".svg",  "image/svg" }
        };

        #endregion

    }
}
