using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Dealer
{

    [Serializable]
    public enum DirectionType
    {
        Send,
        Receive,
        None
    }

    [Serializable]
    public enum DealProtocol
    {
        NONE,
        DOTP,
        HTTP      
    }

    [Serializable]
    public enum ProtocolMethod
    {
        NONE,
        DEAL,
        SYNC,
        GET,
        POST,
        OPTIONS,
        PUT,
        DELETE,
        PATCH
    }

    [Serializable]
    public enum DealComplexity
    {
        Guide,
        Basic,
        Standard,
        Advanced
    }

    [Serializable]
    public enum MessagePart
    {
        Header,
        Message,
    }

   

}
