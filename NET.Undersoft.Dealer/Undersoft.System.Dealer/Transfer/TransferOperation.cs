using System.Text;
using System.Linq;
using System.Collections;
using System.Multemic;
using System.Instants;
using System;

namespace System.Dealer
{   
    public class TransferOperation
    { 
        private DealTransfer transaction;
        private DealContext transportContext;

        private ITransferContext vsspContext;
        private ServiceSite site;
        private DirectionType direction;
        private MessagePart part;
        private DealProtocol protocol;
        private ProtocolMethod method;

        public TransferOperation(DealTransfer _transaction, MessagePart _part, DirectionType _direction)
        {
            transaction = _transaction;
            vsspContext = transaction.Context;
            transportContext = transaction.MyHeader.Context;
            site = transportContext.IdentitySite;
            direction = _direction;
            part = _part;
            protocol = vsspContext.Protocol;
            method = vsspContext.Method;
        }

        public void Resolve(object argument0 = null, object argument1 = null)
        {
            switch (protocol)
            {
                case DealProtocol.DOTP:
                    switch (site)
                    {
                        case ServiceSite.Server:
                            switch (direction)
                            {
                                case DirectionType.Receive:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            SrvRecHead(ref argument0);
                                            break;
                                        case MessagePart.Message:
                                            SrvRecMsg(ref argument0, (int)argument1);
                                            break;
                                    }
                                    break;
                                case DirectionType.Send:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            SrvSendHead();
                                            break;
                                        case MessagePart.Message:
                                            SrvSendMsg();
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case ServiceSite.Client:
                            switch (direction)
                            {
                                case DirectionType.Receive:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            CltRecHead(ref argument0);
                                            break;
                                        case MessagePart.Message:
                                            CltRecMsg(ref argument0, (int)argument1);
                                            break;
                                    }
                                    break;
                                case DirectionType.Send:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            CltSendHead();
                                            break;
                                        case MessagePart.Message:
                                            CltSendMsg();
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            
                case DealProtocol.NONE:              
                    switch (site)
                    {
                        case ServiceSite.Server:
                            switch (direction)
                            {
                                case DirectionType.Receive:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            SrvRecHead(ref argument0);
                                            break;
                                        case MessagePart.Message:
                                            SrvRecMsg(ref argument0, (int)argument1);
                                            break;
                                    }
                                    break;
                                case DirectionType.Send:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            SrvSendHead();
                                            break;
                                        case MessagePart.Message:
                                            SrvSendMsg();
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case ServiceSite.Client:
                            switch (direction)
                            {
                                case DirectionType.Receive:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            CltRecHead(ref argument0);
                                            break;
                                        case MessagePart.Message:
                                            CltRecMsg(ref argument0, (int)argument1);
                                            break;
                                    }
                                    break;
                                case DirectionType.Send:
                                    switch (part)
                                    {
                                        case MessagePart.Header:
                                            CltSendHead();
                                            break;
                                        case MessagePart.Message:
                                            CltSendMsg();
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case DealProtocol.HTTP:
                    switch(method)
                    {
                        case ProtocolMethod.GET:
                            switch (site)
                            {
                                case ServiceSite.Server:
                                    switch (direction)
                                    {
                                        case DirectionType.Receive:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                    SrvRecGet(ref argument0);
                                                    break;                                              
                                            }
                                            break;
                                        case DirectionType.Send:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                    SrvSendGet();
                                                    break;                                               
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case ProtocolMethod.POST:
                            switch (site)
                            {
                                case ServiceSite.Server:
                                    switch (direction)
                                    {
                                        case DirectionType.Receive:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                     SrvRecPost(ref argument0);
                                                    break;
                                            }
                                            break;
                                        case DirectionType.Send:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                    SrvSendPost();
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case ProtocolMethod.OPTIONS:
                            switch (site)
                            {
                                case ServiceSite.Server:
                                    switch (direction)
                                    {
                                        case DirectionType.Receive:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                    SrvRecOptions();
                                                    break;
                                            }
                                            break;
                                        case DirectionType.Send:
                                            switch (part)
                                            {
                                                case MessagePart.Header:
                                                    SrvSendOptions();
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        private void SrvRecHead(ref object array)
        {
            bool isError = false;
            string errorMessage = "";
            try
            {
                DealHeader headerObject = (DealHeader)transaction.MyHeader.Deserialize(ref array);
                if (headerObject != null)
                {
                    transaction.HeaderReceived =  headerObject;

                    if (DealServer.Security.Register(transaction.HeaderReceived.Context.Identity, true))
                    {
                        transaction.MyHeader.Context.Identity.UserId = transaction.HeaderReceived.Context.Identity.UserId;
                        transaction.MyHeader.Context.Identity.Token = transaction.HeaderReceived.Context.Identity.Token;
                        transaction.MyHeader.Context.Identity.DeptId = transaction.HeaderReceived.Context.Identity.DeptId;

                        if (transaction.HeaderReceived.Context.ContentType != null)
                        {

                            object _content = transaction.HeaderReceived.Content;
                            //((ISettings)_content).State.Synced = false;

                            Type[] ifaces = _content.GetType().GetInterfaces();
                            if (ifaces.Contains(typeof(IFigureFormatter)) && ifaces.Contains(typeof(IDealSource)))
                            {
                                int objectCount = transaction.HeaderReceived.Context.ObjectsCount;
                                vsspContext.Synchronic = transaction.HeaderReceived.Context.Synchronic;

                                object myheader = ((IDealSource)_content).Locate();

                                if (myheader != null)
                                {
                                    if (objectCount == 0)
                                    {
                                        vsspContext.ReceiveMessage = false;

                                        //if (((ISettings)_content).Config.DealerId.IsNotEmpty)
                                        //    transaction.MyHeader.Content = ((IDealSource)myheader).Impact(_content);
                                        //else
                                        //{
                                        //    ((ISettings)myheader).State.Expeled = true;
                                            transaction.MyHeader.Content = myheader;
                                       // }
                                    }
                                    else
                                    {
                                        transaction.MyHeader.Content = ((IDealSource)myheader).Impact(_content);
                                        transaction.MessageReceived = new DealMessage(transaction, DirectionType.Receive, transaction.MyHeader.Content);
                                    }
                                }
                                else
                                {
                                    isError = true;
                                    errorMessage += "Prime not exist - incorrect object target ";
                                }
                            }
                            else
                            {
                                isError = true;
                                errorMessage += "Incorrect DPOT object - deserialization error ";
                            }
                        }
                        else
                        {
                            transaction.MyHeader.Content = new Hashtable() { { "Register", true } };
                            transaction.MyHeader.Context.Echo += "Registration success - ContentType: null ";
                        }
                    }
                    else
                    {
                        isError = true;
                        transaction.MyHeader.Content = new Hashtable() { { "Register", false } };
                        transaction.MyHeader.Context.Echo += "Registration failed - access denied ";
                    }
                }
                else
                {
                    isError = true;
                    errorMessage += "Incorrect DPOT object - deserialization error ";
                }
            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage += ex.ToString();
            }

            if (isError)
            {
                transaction.Context.Close = true;
                transaction.Context.ReceiveMessage = false;
                transaction.Context.SendMessage = false;

                if (errorMessage != "")
                {
                    transaction.MyHeader.Content += errorMessage;
                    transaction.MyHeader.Context.Echo += errorMessage;
                }
                transaction.MyHeader.Context.Errors++;
            }
        }
        private void SrvRecMsg(ref object array, int readposition)
        {
            object serialCardsObj = ((object[])transaction.MessageReceived.Content)[readposition];
            object deserialCardsObj = ((IFigureFormatter)serialCardsObj).Deserialize(ref array);
            IFigureFormatter deserialCards = (IFigureFormatter)deserialCardsObj;
            if (deserialCards.DeserialCount <= deserialCards.ProgressCount || deserialCards.ProgressCount == 0)
            {
                transaction.Context.ObjectsLeft--;
                deserialCards.ProgressCount = 0;               
            }
            array = null;
        }

        private void SrvSendHead()
        {
            transaction.Manager.HeaderContent(vsspContext.Transfer.MyHeader.Content,
                                              vsspContext.Transfer.MyHeader.Content, 
                                              DirectionType.Send);

            if (transaction.MyHeader.Context.ObjectsCount == 0)
                vsspContext.SendMessage = false;

            vsspContext.Transfer.MyHeader.Serialize(vsspContext, 0, 0);
        }
        private void SrvSendMsg()
        {            
            int serialBlockId = ((IFigureFormatter[])transaction.MyMessage.Content)[vsspContext.ObjectPosition].Serialize(vsspContext, vsspContext.SerialPacketId, 5000);

            if (serialBlockId < 0)
            {
                if (vsspContext.ObjectPosition < (transaction.MyHeader.Context.ObjectsCount - 1))
                {
                    vsspContext.ObjectPosition++;
                    vsspContext.SerialPacketId = 0;
                    return;
                }           
            }
            vsspContext.SerialPacketId = serialBlockId;
        }

        private void CltRecHead(ref object array)
        {
            DealHeader headerObject = (DealHeader)transaction.MyHeader.Deserialize(ref array);

            if (headerObject != null)
            {
                transaction.HeaderReceived = headerObject;

                transaction.MyHeader.Context.Identity.Key = null;
                transaction.MyHeader.Context.Identity.Name = null;
                transaction.MyHeader.Context.Identity.UserId = transaction.HeaderReceived.Context.Identity.UserId;
                transaction.MyHeader.Context.Identity.Token = transaction.HeaderReceived.Context.Identity.Token;
                transaction.MyHeader.Context.Identity.DeptId = transaction.HeaderReceived.Context.Identity.DeptId;

                object reciveContent = transaction.HeaderReceived.Content;

                Type[] ifaces = reciveContent.GetType().GetInterfaces();
                if (ifaces.Contains(typeof(IFigureFormatter)) && ifaces.Contains(typeof(IDealSource)))
                {
                    if (transaction.MyHeader.Content == null)
                        transaction.MyHeader.Content = ((IDealSource)reciveContent).Locate();

                    object myContent = transaction.MyHeader.Content;

                    ((IDealSource)myContent).Impact(reciveContent);

                    int objectCount = transaction.HeaderReceived.Context.ObjectsCount;
                    if (objectCount == 0)
                        vsspContext.ReceiveMessage = false;
                    else
                        transaction.MessageReceived = new DealMessage(transaction, DirectionType.Receive, myContent);
                }
                else if(reciveContent is Hashtable)
                {
                    Hashtable hashTable = (Hashtable)reciveContent;
                    if (hashTable.Contains("Register"))
                    {
                        vsspContext.Denied = !(bool)hashTable["Register"];
                        if (vsspContext.Denied)
                        {
                            vsspContext.Close = true;
                            vsspContext.ReceiveMessage = false;
                            vsspContext.SendMessage = false;
                        }
                    }
                }
                else
                    vsspContext.SendMessage = false;
            }
        }
        private void CltRecMsg(ref object array, int readposition)
        {
            object serialCardsObj = ((object[])transaction.MessageReceived.Content)[readposition];
            IFigureFormatter serialCards = (IFigureFormatter)serialCardsObj;

            //if (serialCards.ProgressCount == 0)
            //    ((ISettings)serialCardsObj).State.withPropagate = false;

            object deserialCardsObj = serialCards.Deserialize(ref array);
            IFigureFormatter deserialCards = (IFigureFormatter)deserialCardsObj;
            if (deserialCards.DeserialCount <= deserialCards.ProgressCount || deserialCards.ProgressCount == 0)
            {
                transaction.Context.ObjectsLeft--;
                deserialCards.ProgressCount = 0;
                //((ISettings)deserialCardsObj).State.withPropagate = true;
                //((ISettings)((IDeck<)deserialCardsObj).Deck64).State.Emulate(((ISettings)deserialCardsObj).State, true, true);
                //((ISettings)((ICards)deserialCardsObj).Deck64).State.Synced = true;
            }
            array = null;
        }

        private void CltSendHead()
        {
            transaction.Manager.HeaderContent(vsspContext.Transfer.MyHeader.Content, 
                                              vsspContext.Transfer.MyHeader.Content, 
                                              DirectionType.Send);

            if (transaction.MyHeader.Context.ObjectsCount == 0)
                vsspContext.SendMessage = false;

            vsspContext.Transfer.MyHeader.Serialize(vsspContext, 0, 0);
        }
        private void CltSendMsg()
        {
            object serialcards = ((object[])transaction.MyMessage.Content)[vsspContext.ObjectPosition];

            //((ISettings)serialcards).State.Synced = false;
            int serialBlockId = ((IFigureFormatter)serialcards).Serialize(vsspContext, vsspContext.SerialPacketId, 5000);
            if (serialBlockId < 0)
            {
                if (vsspContext.ObjectPosition < (transaction.MyHeader.Context.ObjectsCount - 1))
                {
                    vsspContext.ObjectPosition++;
                    vsspContext.SerialPacketId = 0;
                    return;
                }
            }
            vsspContext.SerialPacketId = serialBlockId;
        }

        private void SrvRecGet(ref object array)
        {
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
            transaction.HeaderReceived = transaction.MyHeader;        
            vsspContext.HandleGetRequest();
        }
        private void SrvSendGet()
        {
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;           
        }

        private void SrvRecPost(ref object array)
        {
            if (SrvRecPostDealer(ref array))
                transaction.HeaderReceived = transaction.MyHeader;
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
        }
        private bool SrvRecPostDealer(ref object array)
        {
            bool isError = false;
            string errorMessage = "";
            try
            {
                byte[] _array = (byte[])array;
                StringBuilder sb = new StringBuilder();
                sb.Append(_array.ToChars(CharEncoding.UTF8));

                string dpttransx = sb.ToString();
                int msgid = dpttransx.IndexOf(",\"DealMessage\":");
                string dptheadx = dpttransx.Substring(0, msgid) + "}";
                string dptmsgx = "{" + dpttransx.Substring(msgid, dpttransx.Length - msgid).Trim(',');

                string[] msgcntsx = dptmsgx.Split(new string[] { "\"Content\":" }, StringSplitOptions.RemoveEmptyEntries);
                string[] cntarrays = msgcntsx.Length > 0 ? msgcntsx[1].Split(new string[] { "\"Cards\":" }, StringSplitOptions.None) : null;
                int objectCount = 0;
                if (cntarrays != null)
                    for (int i = 1; i < cntarrays.Length; i += 1)
                    {
                        string[] itemarray = cntarrays[i].Split('[');
                        for (int x = 1; x < itemarray.Length; x += 1)
                        {
                            if (itemarray[x].IndexOf(']') > 0)
                                objectCount++;
                        }
                    }

                string msgcntx = msgcntsx[1].Trim(' ').Substring(0, 6);

                object dptheadb = dptheadx;
                object dptmsgb = dptmsgx;

                isError = SrvRecPostDealHeader(ref dptheadb, objectCount);
                if(objectCount > 0 && !isError)
                    isError = SrvRecPostDealMessage(ref dptmsgb);

            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage = ex.ToString();
            }

            if (isError)
            {
                transaction.Context.SendMessage = false;
                if (errorMessage != "")
                {
                    transaction.MyHeader.Content += errorMessage;
                    transaction.MyHeader.Context.Echo += errorMessage;
                }
                transaction.MyHeader.Context.Errors++;
            }
            return isError;
        }
        private bool SrvRecPostDealHeader(ref object array, int objcount)
        { 
            bool isError = false;
            string errorMessage = "";
            try
            {
                DealHeader headerObject = (DealHeader)transaction.MyHeader.Deserialize(ref array, FigureFormat.Json);
                headerObject.Context.Identity.Ip = transaction.MyHeader.Context.RemoteEndPoint.Address.ToString();
                if (DealServer.Security.Register(headerObject.Context.Identity, true))
                {
                    transaction.HeaderReceived = (headerObject != null) ? headerObject : null;
                    transaction.MyHeader.Context.Complexity = headerObject.Context.Complexity;
                    transaction.MyHeader.Context.Identity =  headerObject.Context.Identity;

                    if (headerObject.Context.ContentType != null)
                    {

                        object instance = new object();
                        JsonParser.PrepareInstance(out instance, headerObject.Context.ContentType);
                        object content = headerObject.Content;
                        object result = ((IFigureFormatter)instance).Deserialize(ref content, FigureFormat.Json);
                        transaction.HeaderReceived.Content = result;
                        object _content = transaction.HeaderReceived.Content;

                        Type[] ifaces = _content.GetType().GetInterfaces();
                        if (ifaces.Contains(typeof(IFigureFormatter)) && ifaces.Contains(typeof(IDealSource)))
                        {
                            int objectCount = objcount;

                            object myheader = ((IDealSource)_content).Locate();

                            //Settings clientConfig = ((ISettings)_content).Config;

                            if (myheader != null)
                            {
                                if (objectCount == 0)
                                {
                                    vsspContext.ReceiveMessage = false;

                                    //if (clientConfig.DealerId.IsNotEmpty)
                                    //    transaction.MyHeader.Content = ((IDealSource)myheader).Impact(_content);
                                    //else
                                    //{
                                      //  ((ISettings)myheader).State.Expeled = true;
                                        transaction.MyHeader.Content = myheader;
                                   // }
                                }
                                else
                                {
                                    transaction.MyHeader.Content = ((IDealSource)myheader).Impact(_content);
                                    transaction.MessageReceived = new DealMessage(transaction, DirectionType.Receive, transaction.MyHeader.Content);
                                }
                            }
                            else
                            {
                                isError = true;
                                errorMessage += "Prime not exist - incorrect object target ";
                            }
                        }
                        else
                        {
                            isError = true;
                            errorMessage += "Incorrect DPOT object - deserialization error ";
                        }
                    }
                    else
                    {
                        transaction.MyHeader.Content = new Hashtable() { { "Register", true } };
                        transaction.MyHeader.Context.Echo += "Registration success - ContentType: null ";
                    }
                }
                else
                {
                    isError = true;
                    transaction.MyHeader.Content = new Hashtable() { { "Register", false } };
                    transaction.MyHeader.Context.Echo += "Registration failed - access denied ";
                }

            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage += ex.ToString();
            }

            if (isError)
            {
                transaction.Context.SendMessage = false;
                if (errorMessage != "")
                {
                    transaction.MyHeader.Content += errorMessage;
                    transaction.MyHeader.Context.Echo += errorMessage;
                }
                transaction.MyHeader.Context.Errors++;
            }
            return isError;
        }
        private bool SrvRecPostDealMessage(ref object array)
        {
            bool isError = false;
            string errorMessage = "";
            try
            {
              //  ((IFigureFormatter[])transaction.MessageReceived.Content).DeserializeJsonCards(ref array);                
            }
            catch (Exception ex)
            {
                isError = true;
                errorMessage += ex.ToString();
            }

            if (isError)
            {
                transaction.Context.SendMessage = false;
                if (errorMessage != "")
                {
                    transaction.MyHeader.Content = "Prime not exist - incorrect object path";
                    transaction.MyHeader.Context.Echo = "Error - Prime not exist - incorrect object path";
                }
                transaction.MyHeader.Context.Errors++;
            }
            return isError;
        }

        private void SrvSendPost()
        {
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
            vsspContext.RequestBuilder.Clear();
            if (!vsspContext.Denied)
            {
                SrvSendPostDealer();
                vsspContext.HandlePostRequest("application/json");
            }
            else
                vsspContext.HandleDeniedRequest();
        }
        private void SrvSendPostDealer()
        {
            SrvSendPostDealHeader();
            SrvSendPostDealMessage();            
        }
        private void SrvSendPostDealHeader()
        {
            transaction.Manager.HeaderContent(vsspContext.Transfer.MyHeader.Content, vsspContext.Transfer.MyHeader.Content, DirectionType.Send);
            transaction.MyHeader.SetJson(vsspContext.RequestBuilder);
        }
        private void SrvSendPostDealMessage()
        {
            StringBuilder msgcnt = new StringBuilder();

            Type[] ifaces = transaction.MyMessage.Content.GetType().GetInterfaces();
           // if (ifaces.Contains(typeof(ICollection)) && transaction.MyHeader.Context.Errors == 0)
              //  ((IFigureFormatter[])transaction.MyMessage.Content).SerializeJsonCards(msgcnt, 0, 0, transaction.MyHeader.Context.Complexity);
          //  else
                msgcnt.Append("null");

            transaction.MyMessage.Content = new object();
            transaction.MyMessage.SetJson(vsspContext.RequestBuilder);
            string msg = msgcnt.ToString().Replace("}\r\n{", ",").Trim(new char[] { '\n', '\r' });
            vsspContext.RequestBuilder.Replace("\"Content\":{}", "\"Content\":" + msgcnt.ToString());          
            vsspContext.RequestBuilder.Replace("}\r\n{", ",");
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
        }

        private void SrvRecOptions()
        {
            transaction.HeaderReceived = transaction.MyHeader;
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
            vsspContext.HandleOptionsRequest("application/json");
        }
        private void SrvSendOptions()
        {
            vsspContext.SendMessage = false;
            vsspContext.ReceiveMessage = false;
        }
    }
}
