using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Dealer
{
    public class DealManager : IDisposable
    {
        public  ITransferContext transferContext;
        private DealTransfer transfer;
        private DealContext dealContext;
        private ServiceSite site;

        public DealManager(DealTransfer dealTransfer)
        {
            transfer = dealTransfer;
            transferContext = dealTransfer.Context;
            dealContext = dealTransfer.MyHeader.Context;
            site = dealContext.IdentitySite;
        }

        public bool Assign(object content, DirectionType direction, out object[] messages)
        {
            messages = null;

            DealOperation operation = new DealOperation(content, direction, transfer);
            operation.Resolve(out messages);

            if (messages != null)
                return true;
            else
                return false;
        }

        public void Dispose()
        {
            if (transferContext != null)
                transferContext.Dispose();
        }
    }    
}
