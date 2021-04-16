using System.Uniques;
using System.Collections.Generic;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Catalog32
    
    Implementation of Safe-Thread Album abstract class
    using 32 bit hash code and long representation;  
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                      
 
 ******************************************************************/
namespace System.Multemic
{  
    public class Catalog32<V> : SharedAlbum<V>
    {        
        public Catalog32(int capacity = 16) : base(capacity, HashBits.bit32)
        {
        }
        public Catalog32(IList<V> collection, int capacity = 16) : this(capacity > collection.Count ? capacity : collection.Count)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public Catalog32(IEnumerable<V> collection, int capacity = 16) : this(capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }


        public override Card<V> EmptyCard()
        {
            return new Card32<V>();
        }

        public override Card<V> NewCard(long key, V value)
        {
            return new Card32<V>(key, value);
        }
        public override Card<V> NewCard(object key, V value)
        {
            return new Card32<V>(key, value);
        }
        public override Card<V> NewCard(Card<V> card)
        {
            return new Card32<V>(card.Key, card.Value);
        }
        public override Card<V> NewCard(V value)
        {
            return new Card32<V>(base.GetHashKey(value), value);
        }

        public override Card<V>[] EmptyCardTable(int size)
        {
            return new Card32<V>[size];
        }
        public override Card<V>[] EmptyCardList(int size)
        {
            return new Card32<V>[size];
        }
    }   
}
