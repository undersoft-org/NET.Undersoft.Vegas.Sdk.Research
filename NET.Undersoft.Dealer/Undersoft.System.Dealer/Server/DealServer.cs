using System;
using System.Threading;
using System.Instants;

namespace System.Dealer
{
    public class DealServer 
    {
        private IDealListener server;

        public static IMemberSecurity Security;

        public void Start(MemberIdentity ServerIdentity, IMemberSecurity security = null, IDeputy OnEchoEvent = null)
        {            
            server = DealListener.Instance;
            server.Identity = ServerIdentity;
            Security = security;

            new Thread(new ThreadStart(server.StartListening)).Start();

            server.HeaderSent = new DealEvent("HeaderSent", this);
            server.MessageSent = new DealEvent("MessageSent", this);
            server.HeaderReceived = new DealEvent("HeaderReceived", this);
            server.MessageReceived = new DealEvent("MessageReceived", this);
            server.SendEcho = OnEchoEvent;        

            WriteEcho("Dealer instance started");
        }

        public object HeaderReceived(object inetdealcontext)
        {
            string clientEcho = ((ITransferContext)inetdealcontext).Transfer.HeaderReceived.Context.Echo;
            WriteEcho(string.Format("Client header received"));
            if(clientEcho != null && clientEcho != "")
                WriteEcho(string.Format("Client echo: {0}", clientEcho));

            DealContext trctx = ((ITransferContext)inetdealcontext).Transfer.MyHeader.Context;
            if (trctx.Echo == null || trctx.Echo == "")
                trctx.Echo = "Server say Hello";
            if(!((ITransferContext)inetdealcontext).Synchronic)
                server.Send(MessagePart.Header, ((ITransferContext)inetdealcontext).Id);
            else
                server.Receive(MessagePart.Message, ((ITransferContext)inetdealcontext).Id);

            return ((ITransferContext)inetdealcontext);
         }
        public object HeaderSent(object inetdealcontext)
        {

            WriteEcho("Server header sent");

            ITransferContext context = (ITransferContext)inetdealcontext;
            if (context.Close)
            {
                context.Transfer.Dispose();
                server.CloseClient(context.Id);
            }
            else
            {              
                if (!context.Synchronic)
                {
                    if (context.ReceiveMessage)
                        server.Receive(MessagePart.Message, context.Id);
                }
                if (context.SendMessage)
                    server.Send(MessagePart.Message, context.Id);
            }
            return context;
        }

        public object MessageReceived(object inetdealcontext)
        {
            WriteEcho(string.Format("Client message received"));
            if (((ITransferContext)inetdealcontext).Synchronic)
                server.Send(MessagePart.Header, ((ITransferContext)inetdealcontext).Id);
           return (ITransferContext)inetdealcontext;
        }
        public object MessageSent(object inetdealcontext)
        {
            WriteEcho("Server message sent");
            ITransferContext result = (ITransferContext)inetdealcontext;
            if (result.Close)
            {
                result.Transfer.Dispose();                
                server.CloseClient(result.Id);
            }
            return result;
        }

        public void ClearResources()
        {
            WriteEcho("Resource buffer cleaned");
            if(server != null)
             server.ClearResources();
        }
        public void ClearClients()
        {
            WriteEcho("Client registry cleaned");
            if (server != null)
                server.ClearClients();

        }

        public void Close()
        {
            if (server != null)
            {
                WriteEcho("Server instance shutdown ");
                server.CloseListener();
                server = null;
            }
            else
            {
                WriteEcho("Server instance doesn't exist ");
            }
        }

        public bool IsActive()
        {

            if (server != null)
            {
                WriteEcho("Server Instance Is Active");
                return true;
            }
            else
            {
                WriteEcho("Server Instance Doesn't Exist");
                return false;
            }
        }

        public void WriteEcho(string message)
        {
            if(server != null)
                server.Echo(message);
        }

    }
}
