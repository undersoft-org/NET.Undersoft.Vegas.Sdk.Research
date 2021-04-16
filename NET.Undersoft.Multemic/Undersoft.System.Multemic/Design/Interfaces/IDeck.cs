using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
/***************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.IDeck
   
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                     
 
 ***************************************************/
namespace System.Multemic
{
    public interface IDeck<V>: IEnumerable, IEnumerable<V>, IList<V>, IProducerConsumerCollection<V>
    {
        new V this[int index] { get; set; }
        V this[object key] { get; set; }

        Card<V> First { get; }
        Card<V> Last { get; }

        Card<V> Next(Card<V> card);

        new int Count { get; }

        bool ContainsKey(object key);
        bool Contains(Card<V> item);

        V Get(object key);

        bool TryGet(object key, out Card<V> output);
        bool TryGet(object key, out V output);

        Card<V> GetCard(object key);

        bool Add(object key, V value);
        void Add(Card<V> card);
        void Add(IList<Card<V>> cardList);
        void Add(IEnumerable<Card<V>> cards);
        void Add(IList<V> cards);
        void Add(IEnumerable<V> cards);

        bool Enqueue(object key, V value);
        void Enqueue(Card<V> card);
        bool Enqueue(V card);

           V Dequeue();
        bool TryDequeue(out Card<V> item);
        bool TryDequeue(out V item);
        new bool TryTake(out V item);

        Card<V> Put(object key, V value);
        Card<V> Put(Card<V> card);
           void Put(IList<Card<V>> cardList);
           void Put(IEnumerable<Card<V>> cards);
           void Put(IList<V> cards);
           void Put(IEnumerable<V> cards);
           void Put(V value);

              V Remove(object key);
           bool TryRemove(object key);           

            new V[] ToArray();

        new void CopyTo(Array array, int arrayIndex);

          new bool IsSynchronized { get; set; }
        new object SyncRoot { get; set; }

        Card<V> NewCard(V value);
        Card<V> NewCard(object key, V value);

        void CopyTo(Card<V>[] array, int destIndex);

        new void Clear();
    }    
}
