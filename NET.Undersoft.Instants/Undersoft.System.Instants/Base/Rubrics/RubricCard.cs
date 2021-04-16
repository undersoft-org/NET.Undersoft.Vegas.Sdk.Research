using System.Runtime.InteropServices;
using System.Multemic;
using System.Uniques;


namespace System.Instants
{     
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class RubricCard : Card<MemberRubric>
    {
        private long _key;

        public RubricCard()
        { }
        public RubricCard(object key, MemberRubric value) : base(key.GetHashKey64(), value)
        {           
        }
        public RubricCard(long key, MemberRubric value) : base(key, value)
        {
        }
        public RubricCard(Card<MemberRubric> value) : base(value)
        {
        }

        public override void Set(object key, MemberRubric value)
        {
            Value = value;
            _key = key.GetHashKey64();
            Removed = false;
        }
        public override void Set(Card<MemberRubric> card)
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
        public override int CompareTo(Card<MemberRubric> other)
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
