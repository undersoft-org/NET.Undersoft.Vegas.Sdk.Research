using System.Collections;
using System.Collections.Generic;
using System.Uniques;
using System.Multemic.Basedeck;

/*******************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.CardBook   
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                   
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 
 ********************************************************************************/
namespace System.Multemic
{
    public abstract class CardBook<V> : Hashdeck<V>, IEnumerable<V>, IEnumerable, ICollection<V>
    {
        #region Globals       

        protected Card<V>[] list;

        protected Card<V> addNew(long key, V value)
        {
            int id = count + removed;
            var newcard = NewCard(key, value);
            newcard.Index = id;
            list[id] = newcard;
            return newcard;
        }

        public override Card<V> First
        { get { return first; } }
        public override Card<V> Last
        { get { return list[(count + removed) - 1]; } }

        #endregion

        #region Constructor

        public CardBook() : base(16, HashBits.bit64)
        {
            list = EmptyCardList(16);
        }
        public CardBook(int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity, bits)
        {
            list = EmptyCardList(capacity);
        }
        public CardBook(IList<V> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            list = EmptyCardList(capacity);
            foreach (var c in collection)
                this.Add(c);
        }
        public CardBook(IEnumerable<V> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            list = EmptyCardList(capacity);
            foreach (var c in collection)
                this.Add(c);
        }

        #endregion

        #region Operations     

        public override   Card<V> GetCard(int index)
        {
            if (index < count)
            {
                if (removed > 0)
                    Reindex();

                return list[index];
            }
            throw new IndexOutOfRangeException("Index out of range");
        }

        public override   Card<V> InnerPut(long key, V value)
        {
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
                    card.Value = value;
                    if (card.Removed)
                    {
                        card.Removed = false;
                        removedDecrement();
                    }
                    /// update card have same key with new value 
                   
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
                /// assign next conflict card to local card
                card = card.Extent;
            }
        }

        public override      bool InnerAdd(long key, V value)
        {
            // get position index in table, which is an absolute value from key %(modulo) size. Simply it is rest from dividing key by size                           
            ulong pos = getPosition(key);

            Card<V> card = table[pos]; 
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
                    /// assign new card as extent reference and increase conflicts
                    card.Extent = addNew(key, value);
                    conflictIncrement();
                    return true;
                }
                /// assign next conflict card to local card
                card = card.Extent;
            }
        }

        public override      void InnerInsert(int index, Card<V> item)
        {

            int c = count - index;
            if (c > 0)
            {
                // take card at index to replace - removed counter must be 0 otherwise reindex with insert;
                if (removed > 0)
                    reindexWithInsert(index, item);
                else
                {
                   
                    var replaceCard = GetCard(index);

                    while (replaceCard != null)
                    {
                        int id = ++replaceCard.Index;
                        var _replaceCard = list[id];
                        list[id] = replaceCard;
                        replaceCard = _replaceCard;
                    }

                    item.Index = index;
                    list[index] = item;
                }
            }
            else
            {
                // insert card at last position - there is no need to reindex;  
                int id = count + removed;
                item.Index = id;
                list[id] = item;
            }
        }

        public override      void CopyTo(Card<V>[] array, int index)
        {
            int c = count, i = index, l = array.Length;

            if (l - i < c) c = l - i;

            for (int j = 0; j < c; j++)
            {
                array[i++] = GetCard(j);
            }
        }
        public override      void CopyTo(Array array, int index)
        {
            int c = count, i = index, l = array.Length;

            if (l - i < c) c = l - i;

            for (int j = 0; j < c; j++)
                array.SetValue(GetCard(j).Value, i++);
        }
        public override      void CopyTo(V[] array, int index)
        {
            int c = count, i = index, l = array.Length;

            if (l - i < c) c = l - i;

            for (int j = 0; j < c; j++)
                array[i++] = GetCard(j).Value;
        }

        public override       V[] ToArray()
        {
            V[] array = new V[count];
            CopyTo(array, 0);
            return array;
        }

        public override      void Clear()
        {
            base.Clear();
            list = EmptyCardList(minSize);
        }

        public override   Card<V> Next(Card<V> card)
        {
            Card<V> _card = list[card.Index + 1];
            if(_card != null)
            {
                if (!_card.Removed)
                    return _card;
                return Next(_card);
            }
            return null;
        }

        public abstract Card<V>[] EmptyCardList(int size);

        #endregion

        #region Hashtable

        public override void Rehash(int newsize)
        {
            int finish = count;
            int listsize = newsize + (int)(newsize * REMOVED_PERCENT_LIMIT) + 10;
            Card<V>[] newcardTable = EmptyCardTable(newsize);
            if (removed != 0)
            {
                Card<V>[] newcardList = EmptyCardList(listsize);
                rehashAndReindex(newcardTable, newcardList, newsize);
                list = newcardList;
            }
            else
            {
                Card<V>[] newcardList = EmptyCardList(listsize);
                rehash(newcardTable, newsize);
                Array.Copy(list, 0, newcardList, 0, finish);
                list = newcardList;
            }
            table = newcardTable;
            size = newsize;
        }

        private         void rehashAndReindex(Card<V>[] newcardTable, Card<V>[] newcardList, int newsize)
        {
            int _conflicts = 0;
            int _counter = 0;
            int total = count + removed;
            ulong newMixMask = Submix.Mask((ulong)newsize);
            int newMsbId = Submix.MsbId(newsize);
            Card<V> card = null;
            Card<V> mem = null;
            for(int i = 0; i < total; i++ )
            {
                card = list[i];

                if (card != null && !card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize, newMixMask, newMsbId);

                    mem = newcardTable[pos];

                    if (mem == null)
                    {
                        card.Extent = null;
                        card.Index = _counter;
                        newcardTable[pos] = card;
                        newcardList[_counter++] = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extent == null)
                            {
                                card.Extent = null;
                                mem.Extent = card;
                                card.Index = _counter;
                                newcardList[_counter++] = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extent;
                        }
                    }
                }             
            } 
            conflicts = _conflicts;
            removed = 0;
            mixMask = newMixMask;
            msbId = newMsbId;
        }

        private         void rehash(Card<V>[] newcardTable, int newsize)
        {
            int _conflicts = 0;
            int total = count + removed;
            ulong newMixMask = Submix.Mask((ulong)newsize);
            int newMsbId = Submix.MsbId(newsize);
            Card<V> card = null;
            Card<V> mem = null;
            for (int i = 0; i < total; i++)
            {
                card = list[i];
                if (card != null && !card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize, newMixMask, newMsbId);
                    mem = newcardTable[pos];

                    if (mem == null)
                    {
                        card.Extent = null;
                        newcardTable[pos] = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extent == null)
                            {
                                card.Extent = null;
                                mem.Extent = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extent;
                        }
                    }
                }
            } 
            conflicts = _conflicts;
            mixMask = newMixMask;
            msbId = newMsbId;
        }

        public virtual  void Reindex()
        {
            Card<V> card = null;
            int total = count + removed;
            int _counter = 0;
            for (int i = 0; i < total; i++)
            {
                card = list[i];
                if (card != null && !card.Removed)
                {
                    card.Index = _counter;
                    list[_counter++] = card;
                }

            }
            removed = 0;
        }

        private         void reindexWithInsert(int index, Card<V> item)
        {
            Card<V> card = null;
            int _counter = 0;
            int total = count + removed;
            for (int i = 0; i < total; i++)
            {
                card = list[i];
                if (card != null && !card.Removed)
                {
                    card.Index = _counter;
                    list[_counter++] = card;
                    if (_counter == index)
                    {
                        item.Index = _counter;
                        list[_counter++] = item;
                    }
                }

            }
            removed = 0;
        }

        #endregion

    }

}
