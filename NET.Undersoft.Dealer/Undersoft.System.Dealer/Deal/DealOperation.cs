using System.Instants;

namespace System.Dealer
{  

    public class DealOperation : IDisposable
    {
        [NonSerialized]
        public  ITransferContext transferContext;
        private DealContext dealContext;
        private ServiceSite site;
        private DirectionType direction;
        private IFigureFormatter content;
        private ushort state;        

        public DealOperation(object dealContent)
        {                     
            site = ServiceSite.Server;
            direction =  DirectionType.None;
            state = ((ISerialNumber)dealContent).FlagsBlock;
            content = (IFigureFormatter)dealContent;
        }
        public DealOperation(object dealContent, DirectionType directionType, DealTransfer transfer) : this(dealContent)
        {
            direction = directionType;
            transferContext = transfer.Context;
            dealContext = transfer.MyHeader.Context;         
        }

        public void Resolve(out object[] messages)
        {
            messages = null;
            switch (site)
            {
                case ServiceSite.Server:
                    switch (direction)
                    {
                        case DirectionType.Receive:
                           // messages = content.GetMessage();
                            break;
                        case DirectionType.Send:
                            switch (state & (int)StateFlag.Synced)
                            {
                                case 0:
                                    SrvSendSync(out messages);
                                    break;
                            }
                            break;
                        case DirectionType.None:
                            switch (state & (int)StateFlag.Synced)
                            {
                                case 0:
                                    SrvSendSync(out messages);
                                    break;
                            }
                            break;
                    }
                    break;
                case ServiceSite.Client:
                    switch (direction)
                    {
                        case DirectionType.Receive:
                            //messages = content.GetMessage();
                            break;
                        case DirectionType.Send:
                            switch (state & (int)StateFlag.Synced)
                            {
                                case 0:
                                    CltSendSync(out messages);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        private void CltSendSync(out object[] messages)
        {
            if(direction != DirectionType.None)
                if (((state & (int)StateFlag.Edited) > 0) ||
                    ((state & (int)StateFlag.Saved) > 0) ||
                    ((state & (int)StateFlag.Quered) > 0) ||
                    ((state & (int)StateFlag.Canceled) > 0))
                {
                transferContext.Synchronic = true;
                dealContext.Synchronic = true;
            }

            messages = content.GetMessage();
        }

        private void SrvSendSync(out object[] messages)
        {
            if (direction != DirectionType.None)
                if (((state & (int)StateFlag.Edited) > 0) ||
                    ((state & (int)StateFlag.Saved) > 0) ||
                    ((state & (int)StateFlag.Quered) > 0) ||
                    ((state & (int)StateFlag.Canceled) > 0))             
                {
                    transferContext.Synchronic = true;
                    dealContext.Synchronic = true;
                }

            messages = null;
            switch (state & (int)StateFlag.Edited)
            {
                case 2:
                    SrvSendEdit(out messages);
                    break;
            }
            switch (state & (int)StateFlag.Canceled)
            {
                case 32:
                    SrvSendCancel(out messages);
                    break;
            }
            switch ((int)state & (int)StateFlag.Saved)
            {
                case 16:
                    SrvSendSave(out messages);
                    break;
            }
            switch (state & (int)StateFlag.Quered)
            {
                case 8:
                    SrvSendQuery(out messages);
                    break;
            }
            if (messages == null)
            {
               messages = content.GetMessage();              
            }
        }

        private void SrvSendEdit(out object[] messages)
        {
            messages = content.GetMessage();
            /// on edit event
        }
        private void SrvSendSave(out object[] messages)
        {
            messages = content.GetMessage();
            /// on save event
        }
        private void SrvSendCancel(out object[] messages)
        {
            messages = content.GetMessage();
            /// on cancel event
        }
        private void SrvSendQuery(out object[] messages)
        {
            messages = content.GetMessage();
            /// on query event
        }

        public void Dispose()
        {
        }
    }

    public enum StateFlag : ushort
    {
        Synced = 1,
        Edited = 2,
        Added = 4,
        Quered = 8,
        Saved = 16,
        Canceled = 32
    }
}
