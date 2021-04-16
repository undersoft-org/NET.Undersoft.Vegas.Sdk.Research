using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Uniques;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Basedeck.Hashdeck
              
    @author Darius Hanc                                                  
    @project: urcorlib                 
    @version 0.7.1.r.d (Feb 7, 2020)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System.Multemic.Basedeck
{
    public abstract class Hashdeck<V> : Hashkey, ICollection<V>, IList<V>, IDeck<V>, ICollection<Card<V>>, IList<Card<V>>,
                                                 ICollection<IUnique<V>>, IProducerConsumerCollection<V>
    {
        #region Protected       

        static protected readonly float CONFLICTS_PERCENT_LIMIT = 0.35f;
        static protected readonly float REMOVED_PERCENT_LIMIT = 0.15f;
        static protected readonly ulong MAX_BIT_MASK = 0xFFFFFFFFFFFFFFFF;

        protected Card<V> first, last;
        protected Card<V>[] table;
        protected int count, conflicts, removed, minSize, size, primesId, msbId;        
        protected ulong mixMask;      
        
        protected int nextSize()
        {          
            return SIZE_PRIMES.Table[primesId++];
           //return ((size * 2)^3); // Evaluate size without primes
        }

        protected int previousSize()
        {
            return SIZE_PRIMES.Table[primesId++];
            //return (int)(size * (1 - REMOVED_PERCENT_LIMIT)) + 1; // Evaluate size without primes
        }       

        protected void countIncrement()
        {
            if ((++count + 3) > size)               
                Rehash(nextSize());
            // Rehash((((size * 2)^3); // Evaluate size without primes
        }
        protected void conflictIncrement()
        { 
            countIncrement();           
            if (++conflicts > (size * CONFLICTS_PERCENT_LIMIT))                
               Rehash(nextSize());
            //  Rehash((((size * 2)^3); // Evaluate size without primes
        }
        protected void removedIncrement()
        {
            --count;            
            if (++removed > (size * REMOVED_PERCENT_LIMIT))
            {
                if (size < size / 2)
                    Rehash(previousSize());
                else
                    Rehash(size);
              //  Rehash((int)(size * (1 - REMOVED_PERCENT_LIMIT)) + 1); // Evaluate size without primes
            }
        }
        protected void removedDecrement()
        {
            ++count;
            --removed;
        }

        #endregion

        #region Constructor

        public Hashdeck(int capacity = 16, HashBits bits = HashBits.bit64) : base(bits)
        {
            size = capacity;
            minSize = capacity;
            table = EmptyCardTable(capacity);
            first = EmptyCard();
            last = first;
            mixMask = Submix.Mask((ulong)capacity);
            msbId = Submix.MsbId(capacity);
        }
        public Hashdeck(IList<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            this.Add(collection);
        }
        public Hashdeck(IEnumerable<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            this.Add(collection);
        }

        #endregion

        #region Settings

        public virtual          Card<V> First
        { get { return first; } }
        public virtual          Card<V> Last
        { get { return last; } }

        public virtual              int Size{ get => size; }
        public virtual              int Count { get => count; }
        public virtual             bool IsReadOnly { get; set; }
        public virtual             bool IsSynchronized { get; set; }
        public virtual           object SyncRoot { get; set; }

        #endregion

        #region Operations

        #region Item

        Card<V>        IList<Card<V>>.this[int index]
        {
            get => GetCard(index);
            set => GetCard(index).Set(value);
        }
        public virtual              V this[int index]
        {
            get => GetCard(index).Value;
            set => GetCard(index).Value = value;
        }
        protected                   V this[long hashkey]
        {
            get { return InnerGet(hashkey); }
            set { InnerPut(hashkey, value); }
        }
        public virtual              V this[object key]
        {
            get { return InnerGet(base.GetHashKey(key)); }
            set { InnerPut(base.GetHashKey(key), value); }
        }

        #endregion

        #region Get

        public virtual            V InnerGet(long key)
        {
            Card<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                        return mem.Value;
                    return default(V);
                }
                mem = mem.Extent;
            }

            return default(V);
        }
        public virtual            V Get(object key)
        {
            return InnerGet(base.GetHashKey(key));
        }

        public virtual         bool InnerTryGet(long key, out Card<V> output)
        {
            output = null;
            Card<V> mem = table[getPosition(key)];
            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                    {
                        output = mem;
                        return true;
                    }
                    return false;
                }
                mem = mem.Extent;
            }
            return false;
        }
        public virtual         bool TryGet(object key, out Card<V> output)
        {
            return InnerTryGet(base.GetHashKey(key), out output);
        }
        public virtual         bool TryGet(object key, out V output)
        {
            output = default(V);
            Card<V> card = null;
            if (InnerTryGet(base.GetHashKey(key), out card))
            {
                output = card.Value;
                return true;
            }
            return false;
        }

        public virtual      Card<V> InnerGetCard(long key)
        {
            Card<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                        return mem;
                    return null;
                }
                mem = mem.Extent;
            }

            return null;
        }
        public virtual      Card<V> GetCard(object key)
        {
            return InnerGetCard(base.GetHashKey(key));
        }
        public abstract     Card<V> GetCard(int index);

        #endregion

        #region Put

        public abstract     Card<V> InnerPut(long key, V value);
        public virtual      Card<V> Put(object key, V value)
        {
            return InnerPut(base.GetHashKey(key), value);
        }
        public virtual      Card<V> Put(Card<V> _card)
        {
            return InnerPut(_card.Key, _card.Value);
        }
        public virtual         void Put(IList<Card<V>> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                var card = cards[i];
                InnerPut(card.Key, card.Value);
            }
        }
        public virtual         void Put(IEnumerable<Card<V>> cards)
        {
            foreach (Card<V> card in cards)
                InnerPut(card.Key, card.Value);
        }
        public virtual         void Put(V value)
        {
            InnerPut(base.GetHashKey(value), value);
        }
        public virtual         void Put(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Put(cards[i]);

            }
        }
        public virtual         void Put(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Put(card);
        }

        #endregion

        #region Add

        public abstract       bool InnerAdd(long key, V value);
        public virtual        bool Add(object key, V value)
        {
            return InnerAdd(base.GetHashKey(key), value);
        }
        public virtual        void Add(Card<V> card)
        {
            InnerAdd(card.Key, card.Value);
        }
        public virtual        void Add(IList<Card<V>> cardList)
        {
            int c = cardList.Count;
            for (int i = 0; i < c; i++)
            {
                var card = cardList[i];
                InnerAdd(card.Key, card.Value);
            }
        }
        public virtual        void Add(IEnumerable<Card<V>> cardTable)
        {
            foreach (Card<V> card in cardTable)
                Add(card);
        }
        public virtual        void Add(V value)
        {
            InnerAdd(base.GetHashKey(value), value);
        }
        public virtual        void Add(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Add(cards[i]);

            }
        }
        public virtual        void Add(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Add(card);
        }
        public virtual        bool TryAdd(V value)
        {
            return InnerAdd(base.GetHashKey(value), value);
        }

        public abstract       void InnerInsert(int index, Card<V> item);
        public virtual        void Insert(int index, Card<V> item)
        {
            // get position index in table, which is an absolute value from key %(modulo) size. Simply it is rest from dividing key and size                           
            long key = item.Key;
            ulong pos = getPosition(key);

            Card<V> card = table[pos]; /// local for last removed item finded   
            // add in case when item doesn't exist and there is no conflict                                                      
            if (card == null)
            {
                card = NewCard(item);
                table[pos] = card;
                InnerInsert(index, card);
                countIncrement();
                return;
            }

            for (; ; )
            {
                /// key check
                if (card.Equals(key))
                {
                    /// when card was removed insert 
                    if (card.Removed)
                    {
                        var newcard = NewCard(item);
                        card.Extent = newcard;
                        InnerInsert(index, newcard);
                        conflictIncrement();
                        return;
                    }
                    throw new Exception("Item exist");

                }
                /// check that all conflicts was examinated and local card is the last one  
                if (card.Extent == null)
                {
                    var newcard = NewCard(item);
                    card.Extent = newcard;
                    InnerInsert(index, newcard);
                    conflictIncrement();
                    return;
                }
                card = card.Extent;
            }
        }
        public virtual        void Insert(int index, V item)
        {
            Insert(index, NewCard(base.GetHashKey(item), item));
        }

        #endregion

        #region Queue

        public virtual       bool Enqueue(V value)
        {
            return InnerAdd(base.GetHashKey(value), value);
        }
        public virtual       bool Enqueue(object key, V value)
        {
            return InnerAdd(base.GetHashKey(key), value);
        }
        public virtual       void Enqueue(Card<V> card)
        {
            InnerAdd(card.Key, card.Value);
        }

        public virtual       bool TryTake(out V output)
        {
            return TryDequeue(out output);
        }
        public virtual          V Dequeue()
        {
            V card = default(V);
            TryDequeue(out card);
            return card;
        }

        public virtual       bool TryDequeue(out V output)
        {
            var _output = Next(first);
            if (_output != null)
            {
                _output.Removed = true;
                removedIncrement();
                output = _output.Value;
                return true;
            }
            output = default(V);
            return false;
        }
        public virtual       bool TryDequeue(out Card<V> output)
        {
            output = Next(first);
            if (output != null)
            {
                output.Removed = true;
                removedIncrement();
                return true;
            }
            return false;
        }
        #endregion

        #region Contains

        protected            bool InnerContainsKey(long key)
        {
            Card<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (!mem.Removed && mem.Equals(key))
                {

                    return true;
                }
                mem = mem.Extent;
            }

            return false;
        }
        public virtual       bool ContainsKey(object key)
        {
            return InnerContainsKey(base.GetHashKey(key));
        }

        public virtual       bool Contains(Card<V> item)
        {
            return InnerContainsKey(item.Key);
        }
        public virtual       bool Contains(V item)
        {
            return InnerContainsKey(base.GetHashKey(item));
        }

        #endregion

        #region Remove

        public virtual         V InnerRemove(long key)
        {
            Card<V> mem = table[getPosition(key)];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                    {
                        mem.Removed = true;
                        removedIncrement();
                        return mem.Value;
                    }
                    return default(V);
                }

                mem = mem.Extent;
            }
            return default(V);
        }
        public virtual      bool Remove(V item)
        {
            return InnerRemove(base.GetHashKey(item)).Equals(default(V)) ? false : true;
        }
        public virtual         V Remove(object key)
        {
            return InnerRemove(base.GetHashKey(key));
        }
        public virtual      bool Remove(Card<V> item)
        {
            return InnerRemove(item.Key).Equals(default(V)) ? false : true;
        }
        public virtual      bool TryRemove(object key)
        {
            return InnerRemove(base.GetHashKey(key)).Equals(default(V)) ? false : true;
        }
        public virtual      void RemoveAt(int index)
        {
            InnerRemove(GetCard(index).Key);
        }

        public virtual      void Clear()
        {
            size = minSize;
            conflicts = 0;
            removed = 0;
            count = 0;
            table = EmptyCardTable(size);
            first = EmptyCard();
            last = first;
            mixMask = Submix.Mask((ulong)minSize);
            msbId = Submix.MsbId(minSize);
        }

        #endregion

        #region Collection     

        public virtual          void CopyTo(Card<V>[] array, int index)
        {
            int c = count, i = index, l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (Card<V> ves in this.AsCards().Take(c))
                    array[i++] = ves;
            }
            else
                foreach (Card<V> ves in this)
                    array[i++] = ves;
        }
        public virtual          void CopyTo(Array array, int index)
        {
            int c = count, i = index, l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (V ves in this.AsValues().Take(c))
                    array.SetValue(ves, i++);
            }
            else
                foreach (V ves in this.AsValues())
                    array.SetValue(ves, i++);
        }
        public virtual          void CopyTo(V[] array, int index)
        {
            int c = count, i = index, l = array.Length;
            if (l - i < c)
            {
                c = l - i;
                foreach (V ves in this.AsValues().Take(c))
                    array[i++] = ves;
            }
            else
                foreach (V ves in this.AsValues())
                    array[i++] = ves;
        }
        public virtual           V[] ToArray()
        {
            return this.AsValues().ToArray();
        }

        public virtual       Card<V> Next(Card<V> card)
        {
            Card<V> _card = card.Next;
            if (_card != null)
            {
                if (!_card.Removed)
                    return _card;
                return Next(_card);
            }
            return null;
        }

        public virtual          void Resize(int size)
        {
            Rehash(size);
        }

        public abstract      Card<V> EmptyCard();

        public abstract      Card<V> NewCard(long key, V value);
        public abstract      Card<V> NewCard(object key, V value);
        public abstract      Card<V> NewCard(Card<V> card);
        public abstract      Card<V> NewCard(V card);

        public abstract   Card<V>[] EmptyCardTable(int size);

        public virtual         int IndexOf(Card<V> item)
        {
            return GetCard(item).Index;
        }
        public virtual         int IndexOf(V item)
        {
            return GetCard(item).Index;
        }

        #endregion

        #endregion

        #region Enumerable

        public virtual               IEnumerable<V> AsValues()
        {
            return (IEnumerable<V>)this;
        }

        public virtual         IEnumerable<Card<V>> AsCards()
        {
            return (IEnumerable<Card<V>>)this;
        }

        public virtual  IEnumerable<IUnique<V>> AsIdentifiers()
        {
            return (IEnumerable<IUnique<V>>)this;
        }

        public virtual         IEnumerator<Card<V>> GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<V>               IEnumerable<V>.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<IUnique<V>>
                        IEnumerable<IUnique<V>>.GetEnumerator()
        {
            return new CardKeySeries<V>(this);
        }

        IEnumerator                     IEnumerable.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        #endregion

        #region Hashtable

        protected       ulong getPosition(long key)
        {
            // standard hashmap method to establish position / index in table

            // return ((ulong)key % (uint)size);

            // author's algorithm to establish position / index in table            
            // based on most significant bit - BSR (or equivalent depending on the cpu type) 
            // alsow project must be compiled in x64 format (default) for x86 format proper C lib compilation of BitScan.dll is needed       

            return Submix.Map(key, size - 1, mixMask, msbId);           
        }
        protected       ulong getPosition(long key, int newsize, ulong newMixMask, int newMsbId)
        {
            // standard hashmap method to establish position / index in table 

            // return ((ulong)key % (uint)newsize);

            // author's algorithm to establish position / index in table            
            // based on most significant bit - BSR (or equivalent depending on the cpu type)
            // alsow project must be compiled in x64 format (default) for x86 format proper C lib compilation of BitScan.dll is needed       

            return Submix.Map(key, newsize - 1, newMixMask, newMsbId);                    
        }

        public virtual  void Rehash(int newSize)
        {
            int finish = count;
            int newsize = newSize;
            Card<V>[] newcardTable = EmptyCardTable(newsize);
            Card<V> card = first;
            card = card.Next;
            if (removed > 0)
            {
                rehashAndReindex(card, newcardTable, newsize);
            }
            else
            {
                rehash(card, newcardTable, newsize);
            }

            table = newcardTable;
            size = newsize;
        }

        private         void rehashAndReindex(Card<V> card, Card<V>[] newcardTable, int newSize)
        {
            int _conflicts = 0;
            int newsize = newSize;
            ulong newMixMask = Submix.Mask((ulong)newsize);
            int newMsbId = Submix.MsbId(newsize);
            Card<V> _firstcard = EmptyCard();
            Card<V> _lastcard = _firstcard;
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize, newMixMask, newMsbId);

                    Card<V> mem = newcardTable[pos];

                    if (mem == null)
                    {
                        card.Extent = null;
                        newcardTable[pos] = _lastcard = _lastcard.Next = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extent == null)
                            {
                                card.Extent = null;;
                                _lastcard = _lastcard.Next = mem.Extent = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extent;
                        }
                    }
                }

                card = card.Next;

            } while (card != null);

            conflicts = _conflicts;
            removed = 0;
            first = _firstcard;
            last = _lastcard;
            mixMask = newMixMask;
            msbId = newMsbId;
        }

        private         void rehash(Card<V> card, Card<V>[] newcardTable, int newSize)
        {
            int _conflicts = 0;
            int newsize = newSize;
            ulong newMixMask = Submix.Mask((ulong)newsize);
            int newMsbId = Submix.MsbId(newsize);
            do
            {
                if (!card.Removed)
                {
                    ulong pos = getPosition(card.Key, newsize, newMixMask, newMsbId);

                    Card<V> mem = newcardTable[pos];

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

                card = card.Next;

            } while (card != null);
            conflicts = _conflicts;
            mixMask = newMixMask;
            msbId = newMsbId;

        }        

        public          void Add(IUnique<V> item)
        {
            InnerAdd(item.GetHashKey(), item.Target);
        }
        public          bool Contains(IUnique<V> item)
        {
            return InnerContainsKey(item.GetHashKey());
        }
        public          void CopyTo(IUnique<V>[] array, int arrayIndex)
        {
           IUnique<V>[] sn = EmptyCardTable(count);

        }
        public          bool Remove(IUnique<V> item)
        {
            return TryRemove(base.GetHashKey(item));
        }
       
        #endregion

    }  
}
