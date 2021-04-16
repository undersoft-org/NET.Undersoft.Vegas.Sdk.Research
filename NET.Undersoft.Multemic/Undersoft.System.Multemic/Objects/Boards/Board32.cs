using System.Uniques;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Board32
    
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                   
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                             
 
 ******************************************************************/
namespace System.Multemic
{  
    public class Board32<V> : SharedDeck<V>
    {        
        public Board32(int _deckSize = 16, HashBits bits = HashBits.bit64) : base(_deckSize, bits)
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
    }   
}
