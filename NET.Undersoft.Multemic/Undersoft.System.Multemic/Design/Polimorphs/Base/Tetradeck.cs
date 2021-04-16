using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Uniques;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Basedeck.Tetradeck
              
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                      
    @version 0.7.1.r.d (Feb 7, 2020)                                            
    @licence MIT                                       
 *********************************************************************************/
namespace System.Multemic.Basedeck
{
    public abstract class Tetradeck<V> : Hashkey, ICollection<V>, IList<V>, IDeck<V>, ICollection<Card<V>>, IList<Card<V>>,
                                                  ICollection<IUnique<V>>, IProducerConsumerCollection<V>
    {
        #region Protected       

        static protected readonly float CONFLICTS_PERCENT_LIMIT = 0.35f;
        static protected readonly float REMOVED_PERCENT_LIMIT = 0.15f;

        protected Card<V> first, last;
        protected TetraTable<V> table;
        protected TetraSize tsize;
        protected TetraCount tcount;
        protected int count, conflicts, removed, minSize, size, primesId;

        protected void countIncrement(int tid)
        {
            count++;
            if ((tcount.Increment(tid) + 3) > size)
                Rehash(tsize.NextSize(tid), tid);
        }
        protected void conflictIncrement(int tid)
        {
            countIncrement(tid);
            if (++conflicts > (size * CONFLICTS_PERCENT_LIMIT))
                Rehash(tsize.NextSize(tid), tid);
        }
        protected void removedIncrement(int tid)
        {
            int _tsize = tsize[tid];
            --count;
            tcount.Decrement(tid);
            if (++removed > (_tsize * REMOVED_PERCENT_LIMIT))
            {
                if (_tsize < _tsize / 2)
                    Rehash(tsize.PreviousSize(tid), tid);
                else
                    Rehash(_tsize, tid);
            }
        }
        protected void removedDecrement()
        {
            ++count;
            --removed;
        }

        #endregion

        #region Constructor

        public Tetradeck(int capacity = 16, HashBits bits = HashBits.bit64) : base(bits)
        {
            size = capacity;
            minSize = capacity;
            tsize = new TetraSize(capacity);
            tcount = new TetraCount();
            table = new TetraTable<V>(this, capacity);
            first = EmptyCard();
            last = first;
        }
        public Tetradeck(IList<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            this.Add(collection);
        }
        public Tetradeck(IEnumerable<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            this.Add(collection);
        }

        #endregion

        #region Settings

        public virtual Card<V> First
        { get { return first; } }
        public virtual Card<V> Last
        { get { return last; } }

        public virtual int Size
        { get { return size; } }
        public virtual int Count { get => count; }
        public virtual bool IsReadOnly { get; set; }
        public virtual bool IsSynchronized { get; set; }
        public virtual object SyncRoot { get; set; }

        #endregion

        #region Operations

        #region Item

        Card<V> IList<Card<V>>.this[int index]
        {
            get => GetCard(index);
            set => GetCard(index).Set(value);
        }
        public virtual V this[int index]
        {
            get => GetCard(index).Value;
            set => GetCard(index).Value = value;
        }
        protected V this[long hashkey]
        {
            get { return InnerGet(hashkey); }
            set { InnerPut(hashkey, value); }
        }
        public virtual V this[object key]
        {
            get { return InnerGet(base.GetHashKey(key)); }
            set { InnerPut(base.GetHashKey(key), value); }
        }

        #endregion

        #region Get

        public virtual V InnerGet(long key)
        {
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);

            Card<V> mem = table[tid, pos];

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
        public virtual V Get(object key)
        {
            return InnerGet(base.GetHashKey(key));
        }

        public virtual bool InnerTryGet(long key, out Card<V> output)
        {
            output = null;
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);

            Card<V> mem = table[tid, pos];
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
        public virtual bool TryGet(object key, out Card<V> output)
        {
            return InnerTryGet(base.GetHashKey(key), out output);
        }
        public virtual bool TryGet(object key, out V output)
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

        public virtual Card<V> InnerGetCard(long key)
        {
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);

            Card<V> mem = table[tid, pos];
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
        public virtual Card<V> GetCard(object key)
        {
            return InnerGetCard(base.GetHashKey(key));
        }
        public abstract Card<V> GetCard(int index);

        #endregion

        #region Put

        public abstract Card<V> InnerPut(long key, V value);
        public virtual Card<V> Put(object key, V value)
        {
            return InnerPut(base.GetHashKey(key), value);
        }
        public virtual Card<V> Put(Card<V> _card)
        {
            return InnerPut(_card.Key, _card.Value);
        }
        public virtual void Put(IList<Card<V>> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                var card = cards[i];
                InnerPut(card.Key, card.Value);
            }
        }
        public virtual void Put(IEnumerable<Card<V>> cards)
        {
            foreach (Card<V> card in cards)
                InnerPut(card.Key, card.Value);
        }
        public virtual void Put(V value)
        {
            InnerPut(base.GetHashKey(value), value);
        }
        public virtual void Put(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Put(cards[i]);

            }
        }
        public virtual void Put(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Put(card);
        }

        #endregion

        #region Add

        public abstract bool InnerAdd(long key, V value);
        public virtual bool Add(object key, V value)
        {
            return InnerAdd(base.GetHashKey(key), value);
        }
        public virtual void Add(Card<V> card)
        {
            InnerAdd(card.Key, card.Value);
        }
        public virtual void Add(IList<Card<V>> cardList)
        {
            int c = cardList.Count;
            for (int i = 0; i < c; i++)
            {
                var card = cardList[i];
                InnerAdd(card.Key, card.Value);
            }
        }
        public virtual void Add(IEnumerable<Card<V>> cardTable)
        {
            foreach (Card<V> card in cardTable)
                Add(card);
        }
        public virtual void Add(V value)
        {
            InnerAdd(base.GetHashKey(value), value);
        }
        public virtual void Add(IList<V> cards)
        {
            int c = cards.Count;
            for (int i = 0; i < c; i++)
            {
                Add(cards[i]);

            }
        }
        public virtual void Add(IEnumerable<V> cards)
        {
            foreach (V card in cards)
                Add(card);
        }
        public virtual bool TryAdd(V value)
        {
            return InnerAdd(base.GetHashKey(value), value);
        }

        public abstract void InnerInsert(int index, Card<V> item);
        public virtual void Insert(int index, Card<V> item)
        {
            // get position index in table, which is an absolute value from key %(modulo) size. Simply it is rest from dividing key and size                           
            long key = item.Key;
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);
            var _table = table[tid];
            Card <V> card = _table[pos];
            if (card == null)
            {
                card = NewCard(item);
                _table[pos] = card;
                InnerInsert(index, card);
                countIncrement((int)tid);
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
                        conflictIncrement((int)tid);
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
                    conflictIncrement((int)tid);
                    return;
                }
                card = card.Extent;
            }
        }
        public virtual void Insert(int index, V item)
        {
            Insert(index, NewCard(base.GetHashKey(item), item));
        }

        #endregion

        #region Queue

        public virtual bool Enqueue(V value)
        {
            return InnerAdd(base.GetHashKey(value), value);
        }
        public virtual bool Enqueue(object key, V value)
        {
            return InnerAdd(base.GetHashKey(key), value);
        }
        public virtual void Enqueue(Card<V> card)
        {
            InnerAdd(card.Key, card.Value);
        }

        public virtual bool TryTake(out V output)
        {
            return TryDequeue(out output);
        }
        public virtual V Dequeue()
        {
            V card = default(V);
            TryDequeue(out card);
            return card;
        }

        public virtual bool TryDequeue(out V output)
        {
            var _output = Next(first);
            if (_output != null)
            {
                _output.Removed = true;
                removedIncrement((int)getTetraId(_output.Key));
                output = _output.Value;
                return true;
            }
            output = default(V);
            return false;
        }
        public virtual bool TryDequeue(out Card<V> output)
        {
            output = Next(first);
            if (output != null)
            {
                output.Removed = true;
                removedIncrement((int)getTetraId(output.Key));
                return true;
            }
            return false;
        }
        #endregion

        #region Contains

        protected bool InnerContainsKey(long key)
        {
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);

            Card<V> mem = table[tid, pos];

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
        public virtual bool ContainsKey(object key)
        {
            return InnerContainsKey(base.GetHashKey(key));
        }

        public virtual bool Contains(Card<V> item)
        {
            return InnerContainsKey(item.Key);
        }
        public virtual bool Contains(V item)
        {
            return InnerContainsKey(base.GetHashKey(item));
        }

        #endregion

        #region Remove

        public virtual V InnerRemove(long key)
        {
            long tid = getTetraId(key);
            int size = tsize[(int)tid];
            long pos = getPosition(key, size);

            Card<V> mem = table[tid, pos];

            while (mem != null)
            {
                if (mem.Equals(key))
                {
                    if (!mem.Removed)
                    {
                        mem.Removed = true;
                        removedIncrement((int)getTetraId(mem.Key));
                        return mem.Value;
                    }
                    return default(V);
                }

                mem = mem.Extent;
            }
            return default(V);
        }
        public virtual bool Remove(V item)
        {
            return InnerRemove(base.GetHashKey(item)).Equals(default(V)) ? false : true;
        }
        public virtual V Remove(object key)
        {
            return InnerRemove(base.GetHashKey(key));
        }
        public virtual bool Remove(Card<V> item)
        {
            return InnerRemove(item.Key).Equals(default(V)) ? false : true;
        }
        public virtual bool TryRemove(object key)
        {
            return InnerRemove(base.GetHashKey(key)).Equals(default(V)) ? false : true;
        }
        public virtual void RemoveAt(int index)
        {
            InnerRemove(GetCard(index).Key);
        }

        public virtual void Clear()
        {
            size = minSize;
            conflicts = 0;
            removed = 0;
            count = 0;
            tcount.ResetAll();
            tsize.ResetAll();
            table = new TetraTable<V>(this, size);
            first = EmptyCard();
            last = first;
        }

        #endregion

        #region Collection     

        public virtual void CopyTo(Card<V>[] array, int index)
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
        public virtual void CopyTo(Array array, int index)
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
        public virtual void CopyTo(V[] array, int index)
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
        public virtual V[] ToArray()
        {
            return this.AsValues().ToArray();
        }

        public virtual Card<V> Next(Card<V> card)
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

        public virtual void Resize(int size, int tid)
        {
            Rehash(size, tid);
        }

        public abstract Card<V> EmptyCard();

        public abstract Card<V> NewCard(long key, V value);
        public abstract Card<V> NewCard(object key, V value);
        public abstract Card<V> NewCard(Card<V> card);
        public abstract Card<V> NewCard(V card);

        public abstract Card<V>[] EmptyCardTable(int size);

        public virtual int IndexOf(Card<V> item)
        {
            return GetCard(item).Index;
        }
        public virtual int IndexOf(V item)
        {
            return GetCard(item).Index;
        }

        #endregion

        #endregion

        #region Enumerable

        public virtual IEnumerable<V> AsValues()
        {
            return (IEnumerable<V>)this;
        }

        public virtual IEnumerable<Card<V>> AsCards()
        {
            return (IEnumerable<Card<V>>)this;
        }

        public virtual IEnumerable<IUnique<V>> AsIdentifiers()
        {
            return (IEnumerable<IUnique<V>>)this;
        }

        public virtual IEnumerator<Card<V>> GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<V> IEnumerable<V>.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        IEnumerator<IUnique<V>>
                        IEnumerable<IUnique<V>>.GetEnumerator()
        {
            return new CardKeySeries<V>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new CardSeries<V>(this);
        }

        #endregion

        #region Hashtable

        protected long getTetraId(long key)
        {
            return ((key & 1L) - ((key & -1L) * 2));
        }
        protected long getPosition(long key)
        {
            if (key < 0)
                return (key * -1L) % size;
            return key % size;
        }
        protected long getPosition(long key, int newsize)
        {
            if (key < 0)
                return (key * -1L) % newsize;
            return key % newsize;
        }

        public virtual void Rehash(int newsize, int tid)
        {
            int finish = tcount[tid];
            int _tsize = tsize[tid];
            Card<V>[] newcardTable = EmptyCardTable(newsize);
            Card<V> card = first;
            card = card.Next;
            if (removed > 0)
            {
                rehashAndReindex(card, newcardTable, newsize);
            }
            else
            {
                rehashOnly(card, newcardTable, newsize);
            }

            table[tid] = newcardTable;
            size = newsize - _tsize;

        }

        private void rehashAndReindex(Card<V> card, Card<V>[] newcardTable, int newsize)
        {
            int _conflicts = 0;
            int _oldconflicts = 0;
            int _removed = 0;
            Card<V> _firstcard = EmptyCard();
            Card<V> _lastcard = _firstcard;
            do
            {
                if (!card.Removed)
                {
                    long pos = getPosition(card.Key, newsize);

                    Card<V> mem = newcardTable[pos];

                    if (mem == null)
                    {
                        if (card.Extent != null)
                            _oldconflicts++;

                        card.Extent = null;
                        newcardTable[pos] = _lastcard = _lastcard.Next = card;
                    }
                    else
                    {
                        for (; ; )
                        {
                            if (mem.Extent == null)
                            {
                                card.Extent = null; ;
                                _lastcard = _lastcard.Next = mem.Extent = card;
                                _conflicts++;
                                break;
                            }
                            else
                                mem = mem.Extent;
                        }
                    }
                }
                else
                    _removed++;

                card = card.Next;

            } while (card != null);
            conflicts -= _oldconflicts;// _conflicts;
            removed -= _removed;
            first = _firstcard;
            last = _lastcard;
        }

        private void rehashOnly(Card<V> card, Card<V>[] newcardTable, int newsize)
        {
            int _conflicts = 0;
            int _oldconflicts = 0;
            do
            {
                if (!card.Removed)
                {
                    long pos = getPosition(card.Key, newsize);

                    Card<V> mem = newcardTable[pos];

                    if (mem == null)
                    {
                        if (card.Extent != null)
                            _oldconflicts++;

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
            conflicts -= _oldconflicts;// _conflicts;

        }

        public void Add(IUnique<V> item)
        {
            InnerAdd(item.GetHashKey(), item.Target);
        }
        public bool Contains(IUnique<V> item)
        {
            return InnerContainsKey(item.GetHashKey());
        }
        public void CopyTo(IUnique<V>[] array, int arrayIndex)
        {
            IUnique<V>[] sn = EmptyCardTable(count);

        }
        public bool Remove(IUnique<V> item)
        {
            return TryRemove(base.GetHashKey(item));
        }

        #endregion

    }

}
