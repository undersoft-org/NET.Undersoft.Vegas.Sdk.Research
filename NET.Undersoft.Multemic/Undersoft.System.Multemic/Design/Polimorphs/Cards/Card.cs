using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

/******************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Card
    
    Card abstract class. 
    Reference type of common used 
    value type Bucket in Hashtables.
    Include properties: 
    Key - long abstract property to implement different
          type fields with hashkey like long, int etc.
    Value - Generic type property to store collection item.
    Next - for one site list implementation. 
    Extent - for one site list hash conflict items
    Removed - flag for removed items to skip before
              removed items limit exceed and rehash
              process executed
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                          
 
 ****************************************/
namespace System.Multemic
{
    [StructLayout(LayoutKind.Sequential)] [Serializable]
    public abstract class Card<V> : IEquatable<Card<V>>, IEquatable<object>,   IEquatable<long>, IComparable<object>, 
                                    IComparable<long>,   IComparable<Card<V>>, IUnique<V>
    {
        public Card()
        { }
        public Card(long key, V value)
        {
            Value = value;
            Key = key;
            Removed = false;
        }
        public Card(Card<V> value)
        {
            Set(value);
        }

        public int Index = -1;
        public bool Removed;
        
        public abstract long Key { get; set; }
        public             V Value;

        public virtual V Target => Value;

        public virtual  void Set(V value)
        {            
            Value = value;
            Removed = false;
        }
        public virtual  void Set(long key, V value)
        {
            Value = value;
            Key = key;
            Removed = false;
        }
        public abstract void Set(object key, V value);
        public abstract void Set(Card<V> card);

        public virtual bool Equals(IUnique other)
        {
            return Key == other.KeyBlock;
        }
        public virtual bool Equals(Card<V> y)
        {
            return this.Equals(y.Key);
        }
        public virtual bool Equals(long key)
        {
            return Key == key;
        }

        public virtual void SetHashKey(long hashcode)
        {
            KeyBlock = hashcode;
        }
        public virtual long GetHashKey()
        {
            return Key;
        }

        public override abstract bool Equals(object y);
 
        public override abstract int GetHashCode();

        public virtual  int CompareTo(IUnique other)
        {
            return (int)(Key - other.GetHashKey());
        }
        public abstract int CompareTo(object other);
        public virtual  int CompareTo(long key)
        {
            return (int)(Key - key);
        }
        public virtual  int CompareTo(Card<V> other)
        {
            return (int) (Key - other.Key);         
        }

        public abstract byte[] GetBytes();
        public abstract byte[] GetKeyBytes();

        public virtual Card<V> Extent { get; set; }
        public virtual Card<V> Next { get; set; }

        public virtual IUnique Empty => throw new NotImplementedException();

        public virtual long KeyBlock { get => Key; set => Key = value; }

        public virtual Type GetUniqueType() { return this.GetType(); }

        public virtual int[] GetKeyIds() { return null; }

        public virtual object[] GetKeyIdValues() { return null; }
    }    
}
