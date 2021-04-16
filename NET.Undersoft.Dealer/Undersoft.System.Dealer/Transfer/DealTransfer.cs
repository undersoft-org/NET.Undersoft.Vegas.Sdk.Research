using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using System.Data;

namespace System.Dealer
{
    public class DealTransfer : IDisposable
    {
        private DealMessage mymessage;

        public MemberIdentity Identity;
        public ITransferContext Context;

        public DealTransfer()
        {
            MyHeader = new DealHeader(this);
            Manager = new TransferManager(this);
            MyMessage = new DealMessage(this, DirectionType.Send, null);
        }
        public DealTransfer(MemberIdentity identity, object message = null, ITransferContext context = null)
        {
            Context = context;
            if (Context != null)
                MyHeader = new DealHeader(this, Context, identity);
            else
                MyHeader = new DealHeader(this, identity);

            Identity = identity;
            Manager = new TransferManager(this);           
            MyMessage = new DealMessage(this, DirectionType.Send, message);
        }
       
        public TransferManager Manager;

        public DealHeader  MyHeader;
        public DealMessage MyMessage
        {
            get
            {
                return mymessage;
            }
            set
            {
                mymessage = value;                
            }
        }

        public DealHeader HeaderReceived;
        public DealMessage MessageReceived;

        public void Dispose()
        {
            if (MyHeader != null)
                MyHeader.Dispose();
            if (mymessage != null)
                mymessage.Dispose();
            if (HeaderReceived != null)
                HeaderReceived.Dispose();
            if (MessageReceived != null)
                MessageReceived.Dispose();
            if(Context != null)
                Context.Dispose();
        }
    }

   
}
