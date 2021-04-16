using System.IO;
using System.Runtime.InteropServices;
using System.Multemic;
using System.Collections;
using System.Collections.Generic;
using System.Uniques;
using System.Instants.Treatment;

namespace System.Instants
{
    [StructLayout(LayoutKind.Sequential)]
    public abstract class FigureSelection : ISelection
    {
        public abstract IFigure this[int index] { get; set; }

        public abstract object this[int index, string propertyName] { get; set; }

        public abstract object this[int index, int fieldId] { get; set; }

        public abstract IFigures Selection { get; set; }

        public abstract IFigures Collection { get; set; }

        public IFigures Exposition { get => Selection.Exposition; set => Selection.Exposition = value; }

        public FigureFilter Filter { get => Selection.Filter; set => Selection.Filter = value; }
        public FigureSort Sort { get => Selection.Sort; set => Selection.Sort = value; }
        public Func<IFigure, bool> Query { get => Selection.Query; set => Selection.Query = value; }

        public int Serialize(Stream tostream, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }
        public int Serialize(IFigurePacket buffor, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(Stream fromstream, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }
        public object Deserialize(ref object fromarray, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }

        public object[] GetMessage()
        {
            return new[] { this };
        }
        public object GetHeader()
        {
            return Collection;
        }

        public void Clear()
        {
            Selection.Clear();
        }
       
        public IFigure NewFigure()
        {
            return Collection.NewFigure();
        }

        public Card<IFigure> Next(Card<IFigure> card)
        {
            return Selection.Next(card);
        }

        public bool ContainsKey(object key)
        {
            return Selection.ContainsKey(key);
        }
        public bool Contains(IFigure item)
        {
            return Selection.Contains(item);
        }
        public bool Contains(Card<IFigure> item)
        {
            return Selection.Contains(item);
        }

        public IFigure Get(object key)
        {
            return Selection.Get(key);
        }

        public bool TryGet(object key, out Card<IFigure> output)
        {
            return Selection.TryGet(key, out output);
        }
        public bool TryGet(object key, out IFigure output)
        {
            return Selection.TryGet(key, out output);
        }

        public Card<IFigure> GetCard(object key)
        {
            return Selection.GetCard(key);
        }

        public void Add(IFigure item)
        {
            IFigure _item = null;
            if (Collection.TryGet(item, out _item))
            {
                if (!ReferenceEquals(_item, item))
                    _item.ValueArray = item.ValueArray;
                Selection.Add(_item);
            }
            else
                Selection.Put(Collection.Put(item, item));
        }
        public bool Add(object key, IFigure item)
        {
            IFigure _item = null;
            if (Collection.TryGet(key, out _item))
            {
                if (!ReferenceEquals(_item, item))
                    _item.ValueArray = item.ValueArray;
                return Selection.Add(key, _item);
            }
            else
               return Selection.TryAdd(Collection.Put(key, item).Value);
        }
        public void Add(Card<IFigure> item)
        {
            Card<IFigure> _item = null;
            if (Collection.TryGet(item.Key, out _item))
            {
                if (!ReferenceEquals(_item.Value, item.Value))
                    _item.Value.ValueArray = item.Value.ValueArray;
                Selection.Add(_item);
            }
            else
                Selection.Put(Collection.Put(item));
        }
        public void Add(IList<Card<IFigure>> cardList)
        {
            foreach (var card in cardList)
                Selection.Add(card);
        }
        public void Add(IEnumerable<Card<IFigure>> cards)
        {
            foreach (var card in cards)
                Add(card);
        }
        public void Add(IList<IFigure> cards)
        {
            foreach (var card in cards)
                Add(card);
        }
        public void Add(IEnumerable<IFigure> cards)
        {
            foreach (var card in cards)
                Add(card);
        }
        public bool TryAdd(IFigure item)
        {
            IFigure _item = null;
            if (Collection.TryGet(item, out _item))
            {
                if (!ReferenceEquals(_item, item))
                    _item.ValueArray = item.ValueArray;
                return Selection.Add(_item, _item);
            }
            else
                return Selection.TryAdd(Collection.Put(item, item).Value);
        }

        public bool Enqueue(object key, IFigure value)
        {
            return Selection.Enqueue(key, value);
        }
        public void Enqueue(Card<IFigure> card)
        {
            Selection.Enqueue(card);
        }
        public bool Enqueue(IFigure card)
        {
            return Selection.Enqueue(card);
        }

        public IFigure Dequeue()
        {
            return Selection.Dequeue();
        }
        public bool TryDequeue(out Card<IFigure> item)
        {
            return Selection.TryDequeue(out item);
        }
        public bool TryDequeue(out IFigure item)
        {
            return Selection.TryDequeue(out item);
        }

        public bool TryTake(out IFigure item)
        {
            return Selection.TryTake(out item);
        }

        public Card<IFigure> Put(object key, IFigure value)
        {            
            return Selection.Put(Collection.Put(key, value));
        }
        public Card<IFigure> Put(Card<IFigure> card)
        {            
            return Selection.Put(Collection.Put(card));
        }

        public void Put(IList<Card<IFigure>> cardList)
        {
            foreach (var card in cardList)
                Put(card);
        }
        public void Put(IEnumerable<Card<IFigure>> cards)
        {
            foreach (var card in cards)
                Put(card);
        }
        public void Put(IList<IFigure> cards)
        {
            foreach (var card in cards)
                Put(card);
        }
        public void Put(IEnumerable<IFigure> cards)
        {
            foreach (var card in cards)
                Put(card);
        }
        public void Put(IFigure value)
        {           
            Selection.Put(Collection.Put(NewCard(value)));
        }

        public IFigure Remove(object key)
        {
            return Selection.Remove(key);
        }
        public bool    Remove(IFigure item)
        {
            if (Selection.Remove(item) != null)
                return true;
            return false;
        }
        public bool TryRemove(object key)
        {
            return Selection.TryRemove(key);
        }
        public void    RemoveAt(int index)
        {
            Selection.RemoveAt(index);
        }

        public IFigure[] ToArray()
        {
            return Selection.ToArray();
        }

        public void CopyTo(IFigure[] array, int arrayIndex)
        {
            Selection.CopyTo(array, arrayIndex);
        }
        public void CopyTo(Array array, int arrayIndex)
        {
            Selection.CopyTo(array, arrayIndex);
        }
        public void CopyTo(Card<IFigure>[] array, int destIndex)
        {
            Selection.CopyTo(array, destIndex);
        }

        public Card<IFigure> NewCard(IFigure value)
        {
            return Selection.NewCard(value);
        }
        public Card<IFigure> NewCard(object key, IFigure value)
        {
            return Selection.NewCard(key, value);
        }

        public int IndexOf(IFigure item)
        {
            return Selection.IndexOf(item);
        }

        public void Insert(int index, IFigure item)
        {
            Collection.Add(item);
            Selection.Insert(index, item);
        }

        public IEnumerator<IFigure> GetEnumerator()
        {
            return Selection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Selection.GetEnumerator();
        }

        public byte[] GetBytes()
        {
            return Selection.GetBytes();
        }

        public byte[] GetKeyBytes()
        {
            return Selection.GetKeyBytes();
        }

        public void SetHashKey(long value)
        {
            Selection.SetHashKey(value);
        }

        public long GetHashKey()
        {
            return Selection.GetHashKey();
        }

        public bool Equals(IUnique other)
        {
            return Selection.Equals(other);
        }

        public int CompareTo(IUnique other)
        {
            return Selection.CompareTo(other);
        }

        public int SerialCount { get; set; }
        public int DeserialCount { get; set; }
        public int ProgressCount { get; set; }

        public int ItemsCount => Selection.Count;

        public int Count => Selection.Count;

        public FigureCard[] Cards => Selection.Cards;

        public MemberRubrics Rubrics { get => Collection.Rubrics; set => Collection.Rubrics = value; }

        public int Length => Selection.Length;

        public Type FigureType { get => Collection.FigureType; set => Collection.FigureType = value; }
        public int FigureSize { get => Collection.FigureSize; set => Collection.FigureSize = value; }

        public Card<IFigure> First => Selection.First;

        public Card<IFigure> Last => Selection.Last;

        public bool IsSynchronized { get => Selection.IsSynchronized; set => Selection.IsSynchronized = value; }
        public object SyncRoot { get => Selection.SyncRoot; set => Selection.SyncRoot = value; }

        public bool IsReadOnly => Selection.IsReadOnly;

        bool ICollection.IsSynchronized => Selection.IsSynchronized;

        object ICollection.SyncRoot => Selection.SyncRoot;

        public object[] ValueArray { get => Selection.ValueArray; set => Selection.ValueArray = value; }

        public Ussn SystemSerialCode { get => Selection.SystemSerialCode; set => Selection.SystemSerialCode = value; }

        public IUnique Empty => Selection.Empty;

        public long KeyBlock { get => Selection.KeyBlock; set => Selection.KeyBlock = value; }

        public Type FiguresType { get => Collection.FiguresType; set => Collection.FiguresType = value; }       

        object IFigure.this[int fieldId] { get => Selection[fieldId]; set => Selection[fieldId] = (IFigure)value; }
        public object this[string propertyName] { get => Selection[propertyName]; set => Selection[propertyName] = value; }
        public IFigure this[object key] { get => Selection[key]; set => Selection[key] = value; }
    }   
}