using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    [Serializable]
    public class MemberIdentity
    {        
        public int Id { get; set; }
        public int Limit { get; set; }
        public int Scale { get; set; }
        public int Port { get; set; }        
        public string Ip { get; set; }
        public string Host { get; set; }
        public string DeptId { get; set; }
        public string UserId { get; set; }
        public string AuthId { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public string Salt { get; set; }
        public string Token { get; set; }
        public string DataPlace { get; set; }
        public DateTime RegisterTime { get; set; }
        public DateTime LastAction { get; set; }
        public DateTime LifeTime { get; set; }
        public bool Active { get; set; }
        public ServiceSite Site;
        public IdentityType Type;
    }

    [Serializable]
    public enum ServiceSite
    {
        Client,
        Server       
    }

     public enum IdentityType
    {
        User,
        Server,
        Service
    }

   

}
