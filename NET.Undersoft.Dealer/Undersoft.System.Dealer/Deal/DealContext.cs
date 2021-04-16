using System.Net;


namespace System.Dealer
{
    [Serializable]
    public class DealContext
    {
        [NonSerialized] private Type contentType;

        public MemberIdentity Identity { get; set; }
        public ServiceSite IdentitySite
        { get; set; } = ServiceSite.Client;
        public DealComplexity Complexity
        { get; set; } = DealComplexity.Standard;
        [NonSerialized] public IPEndPoint LocalEndPoint;
        [NonSerialized] public IPEndPoint RemoteEndPoint;
        public string EventMethod;
        public string EventClass;
        public bool Synchronic
        { get; set; } = false;
        public bool SendMessage
        { get; set; } = true;
        public bool ReceiveMessage
        { get; set; } = true;
        public Type ContentType
        {
            get
            {
                if (contentType == null && ContentTypeName != null)
                    ContentType = Assemblies.GetType(ContentTypeName);
                return contentType;
            }
            set
            {
                if (value != null)
                {
                    ContentTypeName = value.FullName;
                    contentType = value;
                }
            }
        }   
        public string ContentTypeName { get; set; }
        public int ObjectsCount
        { get; set; } = 0;
        public string Echo { get; set; }
        public int Errors { get; set; }
    }
}
