using System;
using System.Instants;

namespace System.Dealer
{
    public interface IDealListener : IDisposable
    {
        MemberIdentity Identity { get; set; }
      //  IMemberSecurity Security { get; set; }

        IDeputy HeaderReceived { get; set; }
        IDeputy MessageReceived { get; set; }
        IDeputy HeaderSent { get; set; }
        IDeputy MessageSent { get; set; }
        IDeputy SendEcho { get; set; }

        void StartListening();

        bool IsConnected(int id);

        void OnConnectCallback(IAsyncResult result);

        void HeaderReceivedCallback(IAsyncResult result);

        void MessageReceivedCallback(IAsyncResult result);

        void Receive(MessagePart messagePart, int id);

        void Send(MessagePart messagePart, int id);

        void HeaderSentCallback(IAsyncResult result);

        void MessageSentCallback(IAsyncResult result);

        void ClearResources();

        void ClearClients();

        void CloseClient(int id);

        void CloseListener();

        void Echo(string message);
    }
}
