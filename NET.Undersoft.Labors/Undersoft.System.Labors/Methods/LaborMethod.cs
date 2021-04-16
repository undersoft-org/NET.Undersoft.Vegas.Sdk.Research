using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Instants;
using System.Multemic;
using System.Uniques;
using System.Extract;
using System.Linq;

namespace System.Labors
{   
  
    public class LaborMethod : Card<IDeputy>
    {
        private long key;

        public LaborMethod()
        { }
        public LaborMethod(object key, IDeputy value) : base(key.GetHashKey64(), value)
        {
        }
        public LaborMethod(long key, IDeputy value) : base(key, value)
        {
        }
        public LaborMethod(IDeputy value)
        {
            Value = value;
        }

        public override long Key { get => key; set => key = value; }

        public override int CompareTo(object other)
        {
            return (int)(KeyBlock - other.GetHashKey());
        }

        public override bool Equals(object y)
        {
           return KeyBlock == y.GetHashKey64();
        }

        public override byte[] GetBytes()
        {
            return Key.GetBytes();
        }

        public override int GetHashCode()
        {
            return Key.GetBytes().BitAggregate64to32().ToInt32();
        }

        public override byte[] GetKeyBytes()
        {
            return Key.GetBytes();
        }

        public override void Set(object key, IDeputy value)
        {
            Key = key.GetHashKey64();
            Value = value;
            Removed = false; 

        }

        public override void Set(Card<IDeputy> card)
        {
            Key = card.Key;
            Value = card.Value;
            Removed = false;
        }

        public override void Set(IDeputy value)
        {
            Value = value;
            Removed = false;
        }

    }


}
