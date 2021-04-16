using System.Uniques;
using System.Collections.Generic;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Catalog64
    
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                             
 
 ******************************************************************/
namespace System.Multemic
{  
    public class Album64<V> : CardBook<V>
    {
        public Album64() : base(16, HashBits.bit64)
        {
        }
        public Album64(int _deckSize = 16) : base(_deckSize, HashBits.bit64)
        {
        }
        public Album64(ICollection<V> collections, int _deckSize = 16) : base(collections, _deckSize, HashBits.bit64)
        {
        }
        public Album64(IEnumerable<V> collections, int _deckSize = 16) : base(collections, _deckSize, HashBits.bit64)
        {
        }

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
            return new Card32<V>(card.Key, card.Value);
        }
    }
}
