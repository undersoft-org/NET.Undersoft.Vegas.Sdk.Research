using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Uniques;
using System.Threading;

/*************************************************************************************
    Copyright (c) 2020 Undersoft

     System.Multemic.SharedDeck
    
    Abstract class for Safe-Thread Deck
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                                     
 *********************************************************************************/
namespace System.Multemic
{
    public abstract class SharedDeck<V> : CardList<V>
    {
        #region Globals

        protected static readonly int WAIT_READ_TIMEOUT = 5000;
        protected static readonly int WAIT_WRITE_TIMEOUT = 5000;
        protected static readonly int WAIT_REHASH_TIMEOUT = 5000;

        protected ManualResetEventSlim waitRead = new ManualResetEventSlim(true, 500);
        protected ManualResetEventSlim waitWrite = new ManualResetEventSlim(true, 500);
        protected ManualResetEventSlim waitRehash = new ManualResetEventSlim(true, 500);
        protected SemaphoreSlim writePass = new SemaphoreSlim(1);

        public int readers;

        protected void acquireRehash()
        {
            if (!waitRehash.Wait(WAIT_REHASH_TIMEOUT))
                throw new TimeoutException("Wait write Timeout");
            waitRead.Reset();
        }
        protected void releaseRehash()
        {
            waitRead.Set();
        }
        protected void acquireReader()
        {
            Interlocked.Increment(ref readers);
            waitRehash.Reset();
            if (!waitRead.Wait(WAIT_READ_TIMEOUT))
                throw new TimeoutException("Wait write Timeout");
        }
        protected void releaseReader()
        {
            if (0 == Interlocked.Decrement(ref readers))
                waitRehash.Set();
        }
        protected void acquireWriter()
        {
            do
            {
                if (!waitWrite.Wait(WAIT_WRITE_TIMEOUT))
                    throw new TimeoutException("Wait write Timeout");
                waitWrite.Reset();
            }
            while (!writePass.Wait(0));
        }
        protected void releaseWriter()
        {
            writePass.Release();
            waitWrite.Set();
        }

        #endregion

        #region Constructor

        public SharedDeck(int capacity = 16, HashBits bits = HashBits.bit64) : base(capacity, bits)
        {
        }
        public SharedDeck(IList<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity > collection.Count ? capacity : collection.Count, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }
        public SharedDeck(IEnumerable<Card<V>> collection, int capacity = 16, HashBits bits = HashBits.bit64) : this(capacity, bits)
        {
            foreach (var c in collection)
                this.Add(c);
        }

        #endregion

        #region Operations

        public override            V InnerGet(long key)
        {
            acquireReader();
            var v = base.InnerGet(key);
            releaseReader();
            return v;
        }

        public override         bool InnerTryGet(long key, out Card<V> output)
        {
            acquireReader();
            var test = base.InnerTryGet(key, out output);
            releaseReader();
            return test;
        }

        public override      Card<V> InnerGetCard(long key)
        {
            acquireReader();
            var card = base.InnerGetCard(key);
            releaseReader();
            return card;
        }

        public override      Card<V> GetCard(int index)
        {
            if (index < count)
            {
                acquireReader();
                if (removed > 0)
                {
                    releaseReader();
                    acquireWriter();
                    Reindex();
                    releaseWriter();
                    acquireReader();
                }

                if (book != null)
                    return book.GetCard(index);

                int i = -1;
                int id = index;
                var card = first.Next;
                for (; ; )
                {
                    if (++i == id)
                    {
                        releaseReader();
                        return card;
                    }
                    card = card.Next;
                }
            }           
            return null;
        }        

        public override      Card<V> InnerPut(long key, V value)
        {
            acquireWriter();
            var temp = base.InnerPut(key, value);
            releaseWriter();
            return temp;
        }

        public override         bool InnerAdd(long key, V value)
        {
            acquireWriter();
            var temp = base.InnerAdd(key, value);
            releaseWriter();
            return temp;
        }
   
        public override         void Insert(int index, Card<V> item)
        {
            acquireWriter();
            base.Insert(index, item);
            releaseWriter();
        }

        public override            V InnerRemove(long key)
        {
            acquireWriter();
            var temp = base.InnerRemove(key);
            releaseWriter();
            return temp;
        }

        public override         bool TryDequeue(out V output)
        {
            acquireWriter();
            var temp = base.TryDequeue(out output);
            releaseWriter();
            return temp;
        }
        public override         bool TryDequeue(out Card<V> output)
        {
            acquireWriter();
            var temp = base.TryDequeue(out output);
            releaseWriter();
            return temp;
        }

        public override          int IndexOf(Card<V> item)
        {
            int id = 0;
            acquireReader();
            id = base.IndexOf(item);
            releaseReader();
            return id;
        }
        public override          int IndexOf(V item)
        {
            int id = 0;
            acquireReader();
            id = base.IndexOf(item);
            releaseReader();
            return id;
        }

        public override          void CopyTo(Card<V>[] array, int index)
        {
            acquireReader();
            base.CopyTo(array, index);
            releaseReader();
        }
        public override          void CopyTo(Array array, int index)
        {
            acquireReader();
            base.CopyTo(array, index);
            releaseReader();
        }
        public override          void CopyTo(V[] array, int index)
        {
            acquireReader();
            base.CopyTo(array, index);
            releaseReader();
        }

        public override           V[] ToArray()
        {
            acquireReader();
            V[] array = base.ToArray();
            releaseReader();
            return array;
        }

        public override          void Clear()
        {
            acquireWriter();
            acquireRehash();

            base.Clear();

            releaseRehash();
            releaseWriter();
        }

        public override          void Rehash(int newsize)
        {
            acquireRehash();
            base.Rehash(newsize);
            releaseRehash();
        }

        public override          void Reindex()
        {          
            acquireRehash();
            base.Reindex();
            releaseRehash();
        }

        #endregion

    }


}
