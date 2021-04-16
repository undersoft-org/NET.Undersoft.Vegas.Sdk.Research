using System;
using System.IO;
using System.Instants;

namespace System.Dealer
{
    public interface IDealClient : IDisposable
    {
        ITransferContext Context { get; set; }

        IDeputy Connected { get; set; }
        IDeputy HeaderSent { get; set; }
        IDeputy MessageSent { get; set; }
        IDeputy HeaderReceived { get; set; }
        IDeputy MessageReceived { get; set; }
      
        void Connect();

        bool IsConnected();

        void Send(MessagePart messagePart);
      
        void Receive(MessagePart messagePart);
              
    }
}
