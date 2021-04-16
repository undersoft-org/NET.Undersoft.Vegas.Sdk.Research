using System.Linq;
using System.Collections.Generic;
using System.Uniques;
using System.Threading;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Board   
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                   
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                        
             
 *********************************************************************************/
namespace System.Multemic
{  
    public class Board<V> : SharedDeck<V>                                                     
    {

        #region Constructor

        public Board(int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity, bits)
        {
        }
        public Board(IList<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public Board(IEnumerable<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        #endregion

        public override   Card<V> EmptyCard()
        {
            return new Card64<V>();
        }
        public override   Card<V> NewCard(long key, V value)
        {
            return new Card64<V>(key, value);
        }
        public override   Card<V> NewCard(object key, V value)
        {
            return new Card64<V>(key, value);
        }
        public override Card<V>[] EmptyCardTable(int size)
        {
            return new Card64<V>[size];
        }
        public override   Card<V> NewCard(V value)
        {
            return new Card64<V>(value.GetHashKey(), value);
        }
        public override   Card<V> NewCard(Card<V> card)
        {
            return new Card64<V>(card.Key, card.Value);
        }

    }

}
