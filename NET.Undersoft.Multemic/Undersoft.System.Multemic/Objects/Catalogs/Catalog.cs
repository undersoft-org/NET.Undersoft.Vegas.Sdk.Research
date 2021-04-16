using System.Collections.Generic;
using System.Uniques;
using System.Threading;

/*******************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Catalog   
    
    Default Implementation of Safe-Thread Catalog class
    using 64 bit hash code and long representation;  

    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 
 ********************************************************************************/
namespace System.Multemic
{
    public class Catalog<V> : Catalog64<V>
    {
        public Catalog(int capacity = 16) : base(capacity) { }
        public Catalog(IList<V> collection, int capacity = 16) : base(collection, capacity) { }
        public Catalog(IEnumerable<V> collection, int capacity = 16) : base(collection, capacity) { }
    }

}
