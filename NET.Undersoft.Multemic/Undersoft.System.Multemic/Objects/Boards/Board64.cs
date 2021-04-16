using System.Collections.Generic;
using System.Uniques;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Board64
    
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                   
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                        
 
 ******************************************************************/
namespace System.Multemic
{  
    public class Board64<V> : SharedDeck<V>
    {

        #region Constructor

        public Board64(int capacity = 16) : base(capacity, HashBits.bit64)
        {
        }
        public Board64(IList<Card<V>> collection, int capacity = 16) : this(capacity > collection.Count ? capacity : collection.Count)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public Board64(IEnumerable<Card<V>> collection, int capacity = 16) : this(capacity)
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
