using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Instants;
using System.Multemic;

namespace System.Dealer
{
    public sealed class DealListener : IDealListener
    {
        private int timeout = 50;

        private bool shutdown = false;

        private readonly ManualResetEvent connectingNotice = new ManualResetEvent(false);

        private readonly Catalog<ITransferContext> clients =
                     new Catalog<ITransferContext>();

        private readonly Catalog<byte[]> resources =
                     new Catalog<byte[]>();

        public IDeputy HeaderSent { get; set; }
        public IDeputy HeaderReceived { get; set; }
        public IDeputy MessageSent { get; set; }
        public IDeputy MessageReceived { get; set; }
        public IDeputy SendEcho { get; set; }

        public static IDealListener Instance { get; } = new DealListener();

        private MemberIdentity identity;
        public MemberIdentity Identity
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
                                     Port = 28465,
                                     Limit = 200,
                                     Scale = 1,
                                     Site = ServiceSite.Server
                                 };
            }
            set
            {
                if (value != null)
                {
                    value.Site = ServiceSite.Server;
                    identity = value;
                }
            }
        }

        private static IMemberSecurity security;
        public static IMemberSecurity Security
        {
            get { return security; } set { security = value;  }
        }

        private DealListener()
        {
        }

        public void StartListening()
        {
            ushort port = Convert.ToUInt16(Identity.Port),
                  limit = Convert.ToUInt16(Identity.Limit);
            IPAddress address = IPAddress.Parse(Identity.Ip);
            IPEndPoint endpoint = new IPEndPoint(address, port);
            shutdown = false;
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    socket.Bind(endpoint);
                    socket.Listen(limit);
                    while (!shutdown)
                    {

                        connectingNotice.Reset();
                        socket.BeginAccept(OnConnectCallback, socket);
                        connectingNotice.WaitOne();
                    }
                }
            }
            catch (SocketException sx)
            {
                Echo(sx.Message);
            }
        }

        private Card<ITransferContext> GetClient(int id)
        {         
            return clients.GetCard(id);
        }

        public bool IsConnected(int id)
        {
            ITransferContext context = GetClient(id).Value;
            if (context != null && context.Listener != null && context.Listener.Connected)
                return !(context.Listener.Poll(timeout * 100, SelectMode.SelectRead) && context.Listener.Available == 0);
            else
                return false;
        }

        public void OnConnectCallback(IAsyncResult result)
        {
            try
            {
                if (!shutdown)
                {
                    ITransferContext context;
                    int id = -1;
                    id = DateTime.Now.Ticks.ToString().GetHashCode();
                    context = new TransferContext(((Socket)result.AsyncState).EndAccept(result), id);
                    context.Transfer = new DealTransfer(identity, null, context);
                    context.Resources = resources;
                    while (true)
                    {
                        if (!clients.Add(id, context))
                        {
                            id = DateTime.Now.Ticks.ToString().GetHashCode();
                            context.Id = id;
                        }
                        else
                            break;
                    }
                    Echo("Client connected. Get Id " + id);
                    context.Listener.BeginReceive(context.HeaderBuffer, 0, context.BufferSize, SocketFlags.None, HeaderReceivedCallback, clients[id]);
                }
                connectingNotice.Set();
            }
            catch (SocketException sx)
            {
                Echo(sx.Message);
            }
        }

        public void Receive(MessagePart messagePart, int id)
        {
            ITransferContext context = GetClient(id).Value;

            AsyncCallback callback = HeaderReceivedCallback;

            if (messagePart != MessagePart.Header && context.ReceiveMessage)
            {
                callback = MessageReceivedCallback;
                context.ObjectsLeft = context.Transfer.HeaderReceived.Context.ObjectsCount;
                context.Listener.BeginReceive(context.MessageBuffer, 0, context.BufferSize, SocketFlags.None, callback, context);
            }
            else
                context.Listener.BeginReceive(context.HeaderBuffer, 0, context.BufferSize, SocketFlags.None, callback, context);
        }
        public void Send(MessagePart messagePart, int id)
        {
            ITransferContext context = GetClient(id).Value;
            if (!IsConnected(context.Id))
                throw new Exception("Destination socket is not connected.");

            AsyncCallback callback = HeaderSentCallback;

            if (messagePart == MessagePart.Header)
            {
                callback = HeaderSentCallback;
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Header, DirectionType.Send);
                request.Resolve();
            }
            else if (context.SendMessage)
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

        public void HeaderReceivedCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            int receive = context.Listener.EndReceive(result);

            if (receive > 0)
                context.IncomingHeader(receive);

            if (context.Protocol == DealProtocol.DOTP)
                DealHeaderReceived(context);
            else if (context.Protocol == DealProtocol.HTTP)
                HttpHeaderReceived(context);
        }
        public void DealHeaderReceived(ITransferContext context)
        {
            if (context.DeserialPacketSize > 0)
            {
                int buffersize = (context.DeserialPacketSize < context.BufferSize) ? (int)context.DeserialPacketSize : context.BufferSize;
                context.Listener.BeginReceive(context.HeaderBuffer, 0, buffersize, SocketFlags.None, HeaderReceivedCallback, context);
            }
            else
            {
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Header, DirectionType.Receive);
                request.Resolve(context.DeserialPacket);

                context.HeaderReceivedNotice.Set();

                try
                {
                    HeaderReceived.Execute(context);
                }
                catch (Exception ex)
                {
                    Echo(ex.Message);
                    CloseClient(context.Id);
                }
            }
        }
        public void HttpHeaderReceived(ITransferContext context)
        {
            if (context.DeserialPacketSize > 0)
            {
                context.Listener.BeginReceive(context.HeaderBuffer, 0, context.BufferSize, SocketFlags.None, HeaderReceivedCallback, context);
            }
            else
            {
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Header, DirectionType.Receive);
                request.Resolve(context.DeserialPacket);

                context.HeaderReceivedNotice.Set();

                try
                {
                    HeaderReceived.Execute(context);
                }
                catch (Exception ex)
                {
                    Echo(ex.Message);
                    CloseClient(context.Id);
                }
            }
        }

        public void HeaderSentCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            try
            {
                int sendcount = context.Listener.EndSend(result);
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }

            if (!context.ReceiveMessage && !context.SendMessage)
            {
                //int _timeout = 0;
                //while (IsConnected(context.Id) && timeout < 10) _timeout++;
                context.Close = true;
            }

            context.HeaderSentNotice.Set();

            try
            {
                HeaderSent.Execute(context);
            }
            catch (Exception ex)
            {
                Echo(ex.Message);
                CloseClient(context.Id);
            }
        }

        public void MessageReceivedCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            MarkupType noiseKind = MarkupType.None;

            int receive = context.Listener.EndReceive(result);

            if (receive > 0)
                noiseKind = context.IncomingMessage(receive);

            if (context.DeserialPacketSize > 0)
            {
                int buffersize = (context.DeserialPacketSize < context.BufferSize) ? (int)context.DeserialPacketSize : context.BufferSize;
                context.Listener.BeginReceive(context.MessageBuffer, 0, buffersize, SocketFlags.None, MessageReceivedCallback, context);
            }
            else
            {
                object received = context.DeserialPacket;
                object readPosition = context.DeserialPacketId;

                if (noiseKind == MarkupType.Block || (noiseKind == MarkupType.End && (int)readPosition < (context.Transfer.HeaderReceived.Context.ObjectsCount - 1)))
                    context.Listener.BeginReceive(context.MessageBuffer, 0, context.BufferSize, SocketFlags.None, MessageReceivedCallback, context);

                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Message, DirectionType.Receive);
                request.Resolve(received, readPosition);

                if (context.ObjectsLeft <= 0 && !context.BatchesReceivedNotice.SafeWaitHandle.IsClosed)
                    context.BatchesReceivedNotice.Set();

                if (noiseKind == MarkupType.End && (int)readPosition >= (context.Transfer.HeaderReceived.Context.ObjectsCount - 1))
                {                 
                    context.BatchesReceivedNotice.WaitOne();
                    context.MessageReceivedNotice.Set();

                    try
                    {
                        MessageReceived.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        Echo(ex.Message);
                        CloseClient(context.Id);
                    }
                }
            }
        }
        public void MessageSentCallback(IAsyncResult result)
        {
            ITransferContext context = (ITransferContext)result.AsyncState;
            try
            {
                int sendcount = context.Listener.EndSend(result);
            }
            catch (SocketException) { }
            catch (ObjectDisposedException) { }

            if (context.SerialPacketId >= 0 || context.ObjectPosition < (context.Transfer.MyHeader.Context.ObjectsCount - 1))
            {
                TransferOperation request = new TransferOperation(context.Transfer, MessagePart.Message, DirectionType.Send);
                request.Resolve();
                context.Listener.BeginSend(context.SerialPacket, 0, context.SerialPacket.Length, SocketFlags.None, MessageSentCallback, context);
            }
            else
            {
                if (context.ReceiveMessage)
                    context.MessageReceivedNotice.WaitOne();

                //int _timeout = 0;
                // while (IsConnected(context.Id) && timeout < 10) _timeout++;
                context.Close = true;

                context.MessageSentNotice.Set();

                try
                {
                    MessageSent.Execute(context);
                }
                catch(Exception ex)
                {
                    Echo(ex.Message);
                    CloseClient(context.Id);
                }
            }
        }

        public void ClearResources()
        {
            resources.Clear();
        }
        public void ClearClients()
        {
            foreach (ITransferContext closeContext in clients.AsValues())
            {
                ITransferContext context = closeContext;

                if (context == null)
                {
                    throw new Exception("Client does not exist.");
                }

                try
                {
                    context.Listener.Shutdown(SocketShutdown.Both);
                    context.Listener.Close();
                }
                catch (SocketException sx)
                {
                    Echo(sx.Message);
                }
                finally
                {
                    context.Dispose();
                    Echo(string.Format("Client disconnected with Id {0}", context.Id));
                }
            }
            clients.Clear();
        }

        public void CloseClient(int id)
        {
            CloseClient(GetClient(id));
        }
        public void CloseClient(Card<ITransferContext> card)
        {
            ITransferContext context = card.Value;

            if (context == null)
            {
                Echo(string.Format("Client {0} does not exist.", context.Id));
            }
            else
            {
                try
                {
                    if (context.Listener != null && context.Listener.Connected)
                    {
                        context.Listener.Shutdown(SocketShutdown.Both);
                        context.Listener.Close();
                    }
                }
                catch (SocketException sx)
                {
                    Echo(sx.Message);
                }
                finally
                {
                    ITransferContext contextRemoved = clients.Remove(context.Id);
                    contextRemoved.Dispose();
                    Echo(string.Format("Client disconnected with Id {0}", context.Id));
                }
            }
        }

        public void CloseListener()
        {
            foreach (ITransferContext closeContext in clients.AsValues())
            {
                ITransferContext context = closeContext;

                if (context == null)
                {
                    Echo(string.Format("Client  does not exist."));
                }
                else
                {
                    try
                    {
                        if (context.Listener != null && context.Listener.Connected)
                        {
                            context.Listener.Shutdown(SocketShutdown.Both);
                            context.Listener.Close();
                        }
                    }
                    catch (SocketException sx)
                    {
                        Echo(sx.Message);
                    }
                    finally
                    {
                        context.Dispose();
                        Echo(string.Format("Client disconnected with Id {0}", context.Id));
                    }
                }
            }
            clients.Clear();
            shutdown = true;
            connectingNotice.Set();
            GC.Collect();
        }     

        public void Dispose()
        {
            foreach (var card in clients.AsCards())
            {
                CloseClient(card);
            }

            connectingNotice.Dispose();
        }

        public void Echo(string message)
        {
            if (SendEcho != null)
                SendEcho.Execute(message);            
        }

      

     
    }
}
