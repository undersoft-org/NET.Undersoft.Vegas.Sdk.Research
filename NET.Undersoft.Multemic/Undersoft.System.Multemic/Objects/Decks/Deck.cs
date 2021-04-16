using System.Collections.Generic;
using System.Uniques;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Deck      

    Default Implementation of Deck class
    using 64 bit hash code and long representation;  
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System.Multemic
{  
    public class Deck<V> : Deck64<V>                                                     
    {
        public Deck(int capacity = 16) : base(capacity) { }
        public Deck(IList<Card<V>> collection, int capacity = 16) : base(collection, capacity) { }
        public Deck(IEnumerable<Card<V>> collection, int capacity = 16) : base(collection, capacity) { }
    }

}
