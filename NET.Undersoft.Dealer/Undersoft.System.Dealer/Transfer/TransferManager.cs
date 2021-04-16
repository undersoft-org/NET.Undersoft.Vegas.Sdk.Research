using System.Instants;
using System.Linq;
using System;

namespace System.Dealer
{

    public class TransferManager
    {
        private DealTransfer transaction;
        private DealContext context;
        private DealManager treatment;                                                                  // Important Field !!! - Dealer Treatment initiatie, filtering, sorting, saving, editing all treatment here. 

        private ITransferContext transferContext;
        private ServiceSite site;
            
        public TransferManager(DealTransfer _transaction)
        {
            transaction = _transaction;
            transferContext = transaction.Context;
            context = transaction.MyHeader.Context;
            site = context.IdentitySite;
            treatment = new DealManager(_transaction);         
        }

        public void HeaderContent(object content, object value, DirectionType _direction)
        {
            DirectionType direction = _direction;
            object _content = value;
            if (_content != null)
            {              
                Type[] ifaces = _content.GetType().GetInterfaces();
                if (ifaces.Contains(typeof(IFigureFormatter)))
                {
                    transaction.MyHeader.Context.ContentType = _content.GetType();

                    if (direction == DirectionType.Send)
                        _content = ((IFigureFormatter)value).GetHeader();

                    object[] messages_ = null;                   
                    if (treatment.Assign(_content, direction, out messages_)                               // Dealer Treatment assign with handle its only place where its called and mutate data. 
                    ){
                        if (messages_.Length > 0)
                        {                         
                            context.ObjectsCount = messages_.Length;
                            for (int i = 0; i < context.ObjectsCount; i++)
                            {
                                IFigureFormatter message = ((IFigureFormatter[])messages_)[i];
                                IFigureFormatter head = (IFigureFormatter)((IFigureFormatter[])messages_)[i].GetHeader();
                                message.SerialCount = message.ItemsCount;
                                head.SerialCount = message.ItemsCount;                            
                            }
                               
                            if (direction == DirectionType.Send)
                                transaction.MyMessage.Content = messages_;
                            else
                                transaction.MyMessage.Content = ((IFigureFormatter)_content).GetHeader();
                        }
                   }                                                                                               
                }
            }
            content = _content;
        }
        public void MessageContent(ref object content, object value, DirectionType _direction)
        {
            DirectionType direction = _direction;
            object _content = value;
            if (_content != null)
            {          
                if (direction == DirectionType.Receive)
                {
                    Type[] ifaces = _content.GetType().GetInterfaces();
                    if (ifaces.Contains(typeof(IFigureFormatter)))
                    {                       
                        object[] messages_ = ((IFigureFormatter)value).GetMessage();
                        if (messages_ != null)
                        {                            
                            int length = messages_.Length;
                            for (int i = 0; i < length; i++)
                            {
                                IFigureFormatter message = ((IFigureFormatter[])messages_)[i];
                                IFigureFormatter head = (IFigureFormatter)((IFigureFormatter[])messages_)[i].GetHeader();
                                message.SerialCount = head.SerialCount;
                                message.DeserialCount = head.DeserialCount;
                            }                          

                            _content = messages_;
                        }
                    }
                }              
            }
            content = _content;
        }
    }   
}
