using System.Collections.Generic;
using System.Uniques;
using System.Multemic.Basedeck;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Deck    
        
    @author Darius Hanc                                                  
    @project  NETStandard.Undersoft.Library                               
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System.Multemic
{
    public abstract class CardList<V> : Hashdeck<V>
    {
        #region Globals       

        protected  CardBook<V> book;

        protected      Card<V> addNew(long key, V value)
        {
            var newcard = NewCard(key, value);
            last.Next = newcard;
            last = newcard;
            return newcard;
        }

        #endregion

        #region Constructor

        public CardList(int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity, bits)
        {
        }
        public CardList(IList<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public CardList(IEnumerable<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        #endregion

        #region Operations

        public override Card<V> GetCard(int index)
        {
            if (index < count)
            {
                if (removed > 0)
                    Reindex();

                if (book != null)
                    return book.GetCard(index);

                int i = -1;
                int id = index;
                var card = first.Next;
                for (; ; )
                {
                    if (++i == id)
                        return card;
                    card = card.Next;
                }
            }
            return null;
        }
       
        public override Card<V> InnerPut(long key, V value)
        {
            // get position index in table, which is an absolute value from key %(modulo) size. Simply it is rest from dividing key and size                           
            ulong pos = getPosition(key);

            Card<V> card = table[pos]; /// local for last removed item finded   
            // add in case when item doesn't exist and there is no conflict                                                      
            if (card == null)
            {
                card = addNew(key, value);
                table[pos] = card;
                countIncrement();
                return card;
            }

            // loop over conflicts assigned as reference in extent field                      
            for (; ; )
            {
                /// key check
                if (card.Equals(key))
                {
                    /// when card was removed decrese counter 
                    if (card.Removed)
                    {
                        card.Removed = false;
                        removedDecrement();
                    }
                    /// update card have same key with new value 
                    card.Value = value;
                    return card;
                }
                /// check that all conflicts was examinated and local card is the last one  
                if (card.Extent == null)
                {
                    var newcard = addNew(key, value);
                    card.Extent = newcard;
                    conflictIncrement();
                    return newcard;
                }
                card = card.Extent;
            }
        }
    
        public override    bool InnerAdd(long key, V value)
        {
            // get position index in table, which is an absolute value from key %(modulo) size. Simply it is rest from dividing key and size                           
            ulong pos = getPosition(key);

            Card<V> card = table[pos]; /// local for last removed item finded   
            // add in case when item doesn't exist and there is no conflict                                                      
            if (card == null)
            {
                table[pos] = addNew(key, value);
                countIncrement();
                return true;
            }

            // loop over conflicts assigned as reference in extent field                      
            for (; ; )
            {
                /// key check
                if (card.Equals(key))
                {
                    /// when card was removed decrese counter 
                    if (card.Removed)
                    {
                        /// update card have same key with new value 
                        card.Removed = false;
                        card.Value = value;
                        removedDecrement();
                        return true;
                    }                                       
                    return false;
                }
                /// check that all conflicts was examinated and local card is the last one  
                if (card.Extent == null)
                {
                    card.Extent = addNew(key, value);
                    conflictIncrement();
                    return true;
                }
                card = card.Extent;
            }
        }

        public override    void InnerInsert(int index, Card<V> item)
        {
            if (index < count - 1)
            {
                if (index == 0)
                {
                    item.Index = 0;
                    item.Next = first.Next;
                    first.Next = item;
                }
                else
                {

                    Card<V> prev = GetCard(index - 1);
                    Card<V> next = prev.Next;
                    prev.Next = item;
                    item.Next = next;
                }
            }
            else
            {
                last = last.Next = item;             
            }
        }

        public virtual     void Reindex()
        {
            Card<V> _firstcard = EmptyCard();
            Card<V> _lastcard = _firstcard;
            Card<V> card = first.Next;
            do
            {
                if (!card.Removed)
                {
                    _lastcard = _lastcard.Next = card;
                }

                card = card.Next;

            } while (card != null);
            removed = 0;
            first = _firstcard;
            last = _lastcard;
        }

        #endregion

    }


}
