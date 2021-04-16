using System.Threading;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Instants;

namespace System.Dealer
{
    public sealed class DealClient : IDealClient
    {
        private Socket socket;
        private ushort port;
        private IPAddress ip;
        private IPHostEntry host;
        private int timeout = 50;

        private readonly ManualResetEvent connectNotice = new ManualResetEvent(false);

        private ITransferContext context;
        public ITransferContext Context
        { get { return context; } set { context = value; } }       

        public IDeputy Connected { get; set; }
        public IDeputy HeaderSent { get; set; }
        public IDeputy MessageSent { get; set; }
        public IDeputy HeaderReceived { get; set; }
        public IDeputy MessageReceived { get; set; }

        private MemberIdentity identity;
        public  MemberIdentity Identity
        {
            get
            {
                return (identity != null) ?
                                 identity :
                                 identity = new MemberIdentity()
                                 {
                                     Id = 0,
                                     Ip = "127.0.0.1",
                                     Host = "localhost",
                                     Port = 44004,
                                     Limit = 0,
                                     Scale = 0,
                                     Site = ServiceSite.Client
                                 };
            }
            set
            {
                if (value != null)
                {
                    value.Site = ServiceSite.Client;
                    identity = value;
                }
            }
        }

        public  IPEndPoint EndPoint;

        public DealClient(MemberIdentity ConnectionIdentity)
        {
            Identity = ConnectionIdentity;

            if(Identity.Ip == null || Identity.Ip == "")
                Identity.Ip = "127.0.0.1";
            ip =   IPAddress.Parse(Identity.Ip);
            port = Convert.ToUInt16(Identity.Port);
            host = Dns.GetHostEntry((Identity.Ip != null &&
                                     Identity.Ip != string.Empty) ?
                                     Identity.Ip :
                                     string.Empty);
            
            EndPoint = new IPEndPoint(ip, port);
        }

        public void Connect()
        {
                   ushort _port = port;
                string hostname = host.HostName;
                  IPAddress _ip = ip;
            IPEndPoint endpoint = new IPEndPoint(_ip, _port);            

            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                context = new TransferContext(socket);
                socket.BeginConnect(endpoint, OnConnectCallback, context);
                connectNotice.WaitOne();

                Connected.Execute(this);
            }
            catch (SocketException ex)
            { }
        }

        public bool IsConnected()
        {
            if(socket != null && socket.Connected)
                return !(socket.Poll(timeout * 1000, SelectMode.SelectRead) && socket.Available == 0);
            return true;
        }

        private void OnConnectCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;        

            try
            {
                context.Listener.EndConnect(result);
                connectNotice.Set();
            }
            catch (SocketException ex)
            {
            }
        }

        public void Receive(MessagePart messagePart)
        {
            AsyncCallback callback = HeaderReceivedCallBack;
            if (messagePart != MessagePart.Header && context.ReceiveMessage)
            {
                callback = MessageReceivedCallBack;
                context.ObjectsLeft = context.Transfer.HeaderReceived.Context.ObjectsCount;
                context.Listener.BeginReceive(context.MessageBuffer, 0, context.BufferSize, SocketFlags.None, callback, context);
            }
            else
                context.Listener.BeginReceive(context.HeaderBuffer, 0, context.BufferSize, SocketFlags.None, callback, context);
        }
        public void Send(MessagePart messagePart)
        {
            if (!IsConnected())
                throw new Exception("Destination socket is not connected."); 
            AsyncCallback callback = HeaderSentCallback;
            if (messagePart == MessagePart.Header)
            {
                callback = HeaderSentCallback;
                TransferOperation request = new TransferOperation(Context.Transfer, MessagePart.Header, DirectionType.Send);
                request.Resolve();
            }
            else if (Context.SendMessage)
            {
                callback = MessageSentCallback;
                context.SerialPacketId = 0;
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Message, DirectionType.Send);
                request.Resolve();
            }
            else
                return;
            context.Listener.BeginSend(context.SerialPacket, 0, context.SerialPacket.Length, SocketFlags.None, callback, context);
        }

        private void MessageReceivedCallBack(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            MarkupType noiseKind = MarkupType.None;

            int receive = context.Listener.EndReceive(result);

            if (receive > 0)
                noiseKind = context.IncomingMessage(receive);

            if (context.DeserialPacketSize > 0)
            {
                int buffersize = (context.DeserialPacketSize < context.BufferSize) ? (int)context.DeserialPacketSize : context.BufferSize;
                context.Listener.BeginReceive(context.MessageBuffer, 0, buffersize, SocketFlags.None, MessageReceivedCallBack, context);
            }
            else
            {
                object received = context.DeserialPacket;
                object readPosition = context.DeserialPacketId;

                if (noiseKind == MarkupType.Block || (noiseKind == MarkupType.End && (int)readPosition < (context.Transfer.HeaderReceived.Context.ObjectsCount - 1)))
                    context.Listener.BeginReceive(context.MessageBuffer, 0, context.BufferSize, SocketFlags.None, MessageReceivedCallBack, context);

                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Message, DirectionType.Receive);
                request.Resolve(received, readPosition);

                if (context.ObjectsLeft <= 0 && !context.BatchesReceivedNotice.SafeWaitHandle.IsClosed)
                    context.BatchesReceivedNotice.Set();

                if (noiseKind == MarkupType.End && (int)readPosition >= (context.Transfer.HeaderReceived.Context.ObjectsCount - 1))
                {
                    context.BatchesReceivedNotice.WaitOne();

                    if (context.SendMessage)          
                        context.MessageSentNotice.WaitOne();
                    
                    context.Close = true;

                    context.MessageReceivedNotice.Set();
                    MessageReceived.Execute(this);
                }
            }
        }
        private void MessageSentCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            try
            {
                int sendcount = context.Listener.EndSend(result);               
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }

            if (context.SerialPacketId >= 0)
            {
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Message, DirectionType.Send);
                request.Resolve();
                context.Listener.BeginSend(context.SerialPacket, 0, context.SerialPacket.Length, SocketFlags.None, MessageSentCallback, context);
            }
            else
            {
                if (!context.ReceiveMessage)
                    context.Close = true;

                context.MessageSentNotice.Set();
                MessageSent.Execute(this);
            }
        }

        private void HeaderReceivedCallBack(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            int receive = context.Listener.EndReceive(result);

            if (receive > 0)
                context.IncomingHeader(receive);

            if (context.DeserialPacketSize > 0)
            {
                int buffersize = (context.DeserialPacketSize < context.BufferSize) ? (int)context.DeserialPacketSize : context.BufferSize;
                context.Listener.BeginReceive(context.HeaderBuffer, 0, buffersize, SocketFlags.None, HeaderReceivedCallBack, context);
            }
            else
            {
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Header, DirectionType.Receive);
                request.Resolve(context.DeserialPacket);

                if (!context.ReceiveMessage &&
                    !context.SendMessage)
                    context.Close = true;

                context.HeaderReceivedNotice.Set();
                HeaderReceived.Execute(this);
            }
        }
        private void HeaderSentCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            try
            {
                int sendcount = context.Listener.EndSend(result);
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }

            context.HeaderSentNotice.Set();
            HeaderSent.Execute(this);
        }

        private void Close()
        {
            try
            {
                if (!IsConnected())
                {
                    context.Dispose();
                    return;
                }
                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                context.Dispose();
            }
            catch (SocketException)
            {
                // 4U2DO
            }
        }

        public void Dispose()
        {
            connectNotice.Dispose();
            Close();
        }      
    }
}
