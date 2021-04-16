using System.Runtime.InteropServices;
using System.Uniques;

/******************************************************************
    Copyright (c) 2020 Undersoft

    System.Multemic.Card64
    
    Implementation of Card abstract class
    using 64 bit hash code and long representation;  
        
    @author Darius Hanc                                                  
    @project NETStandard.Undersoft.SDK                                    
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 
 ******************************************************************/
namespace System.Multemic
{     
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Card64<V> : Card<V>
    {
        private long _key;

        public Card64()
        { }
        public Card64(object key, V value) : base(key.GetHashKey64(), value)
        {
        }
        public Card64(long key, V value) : base(key, value)
        {
        }
        public Card64(Card<V> value) : base(value)
        {
        }

        public override void Set(object key, V value)
        {
            Value = value;
            _key = key.GetHashKey64();
            Removed = false;
        }
        public override void Set(Card<V> card)
        {
            Value = card.Value;
            _key = card.Key;
            Removed = false;
        }

        public override bool Equals(long key)
        {
            return Key == key;
        }
        public override bool Equals(object y)
        {
            return Key.Equals(y.GetHashKey64());
        }

        public override int GetHashCode()
        {
            return (int)Key;
        }

        public override int CompareTo(object other)
        {
            return (int)(Key - other.GetHashKey64());
        }
        public override int CompareTo(long key)
        {
            return (int)(Key - key);
        }
        public override int CompareTo(Card<V> other)
        {
            return (int)(Key - other.Key);
        }

        public override byte[] GetBytes()
        {
            return GetKeyBytes();
        }

        public unsafe override byte[] GetKeyBytes()
        {
            byte[] b = new byte[8];
            fixed (byte* s = b)
                *(long*)s = _key;
            return b;
        }

        public override long Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
    }
}
