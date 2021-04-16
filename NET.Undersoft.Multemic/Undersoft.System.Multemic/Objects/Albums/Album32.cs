using System.Uniques;
using System.Collections.Generic;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Catalog32   
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                            
 
 ******************************************************************/
namespace System.Multemic
{  
    public class Album32<V> : CardBook<V>
    {
        public Album32() : base(16, HashBits.bit32)
        { 
        } 
        public Album32(int _deckSize = 8) : base(_deckSize, HashBits.bit32)
        {
        }
        public Album32(ICollection<V> collections, int _deckSize = 8) : base(collections, _deckSize, HashBits.bit32)
        {
        }
        public Album32(IEnumerable<V> collections, int _deckSize = 8) : base(collections, _deckSize, HashBits.bit32)
        {
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

        public override Card<V>[] EmptyCardTable(int size)
        {
            return new Card32<V>[size];
        }

        public override Card<V>[] EmptyCardList(int size)
        {
            return new Card32<V>[size];
        }

        public override Card<V> NewCard(V value)
        {
            return new Card32<V>(value.GetHashKey(), value);
        }

        public override Card<V> NewCard(Card<V> card)
        {
            return new Card32<V>(card.Key, card.Value);
        }
    }
}
