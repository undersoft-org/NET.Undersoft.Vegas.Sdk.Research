using System.Uniques;
using System.Collections.Generic;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Catalog64
    
    Implementation of Safe-Thread Album abstract class
    using 64 bit hash code and long representation;  
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT
 ******************************************************************/
namespace System.Multemic
{  
    public class Catalog64<V> : SharedAlbum<V>
    {
        #region Constructor

        public Catalog64(int capacity = 16) : base(capacity, HashBits.bit64)
        {
        }
        public Catalog64(IList<V> collection, int capacity = 16) : this(capacity > collection.Count ? capacity : collection.Count)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public Catalog64(IEnumerable<V> collection, int capacity = 16) : this(capacity)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        #endregion        

        public override Card<V> EmptyCard()
        {
            return new Card64<V>();
        }
        public override Card<V> NewCard(long key, V value)
        {
            return new Card64<V>(key, value);
        }
        public override Card<V> NewCard(object key, V value)
        {
            return new Card64<V>(key, value);
        }
        public override Card<V>[] EmptyCardTable(int size)
        {
            return new Card64<V>[size];
        }
        public override Card<V>[] EmptyCardList(int size)
        {
            return new Card64<V>[size];
        }
        public override Card<V> NewCard(V value)
        {
            return new Card64<V>(value.GetHashKey(), value);
        }
        public override Card<V> NewCard(Card<V> card)
        {
            return new Card64<V>(card.Key, card.Value);
        }
    }   
}
