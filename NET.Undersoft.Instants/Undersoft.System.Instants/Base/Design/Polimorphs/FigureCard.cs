using System.Runtime.InteropServices;
using System.Extract;
using System.Uniques;
using System.Multemic;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

/******************************************************************
    Copyright (c) 2020 Undersoft

    @name System.Instants.FigureCard                
    
    @project NET.Undersoft.Sdk
    @author Darius Hanc                                                                               
    @version 0.8.D (Feb 7, 2020)                                            
    @licence MIT                                       
 
 ******************************************************************/
namespace System.Instants
{     
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class FigureCard : Card<IFigure>, IFigure, IEquatable<IFigure>, IComparable<IFigure>
    {       
        public FigureCard()
        { }
        public FigureCard(object key, IFigure value) : base(key.GetHashKey(), value)
        {
        }
        public FigureCard(long key, IFigure value) : base(key, value)
        {
        }
        public FigureCard(IFigure value)
        {
            Value = value;
            Removed = false;
        }
        public FigureCard(Card<IFigure> value)
        {
            Set(value);
        }

        public object this[int fieldId]
        {
            get => GetPreset(fieldId);
            set => SetPreset(fieldId, value);
        }
        public object this[string propertyName]
        {
            get => GetPreset(propertyName);
            set => SetPreset(propertyName, value);
        }

        public override void Set(object key, IFigure value)
        {                        
            Value = value;
            value.KeyBlock = key.GetHashKey();
            Removed = false;
        }
        public override void Set(Card<IFigure> card)
        {
            Value = card.Value;
            Removed = false;
        }

        public override bool Equals(long key)
        {
            return Key == key;
        }
        public override bool Equals(object y)
        {
            return Key.Equals(y.GetHashKey());
        }
        public          bool Equals(IFigure other)
        {
            return Key == other.KeyBlock;
        }

        public override int GetHashCode()
        {
            return Value.GetKeyBytes().BitAggregate64to32().ToInt32();
        }

        public override int CompareTo(object other)
        {
            return (int)(Key - other.GetHashKey64());
        }
        public override int CompareTo(long key)
        {
            return (int)(Key - key);
        }
        public override int CompareTo(Card<IFigure> other)
        {
            return (int)(Key - other.Key);
        }
        public          int CompareTo(IFigure other)
        {
            return (int)(Key - other.KeyBlock);
        }

        public override byte[] GetBytes()
        {
            IFigure f = null;
            if (Presets != null)
            {
                f = Collection.NewFigure();
                f.ValueArray = ValueArray;
                f.SystemSerialCode = SystemSerialCode;
            }
            else
                f = Value;
            return f.GetBytes();
        }

        public unsafe override byte[] GetKeyBytes()
        {
            byte[] b = new byte[8];
            fixed (byte* s = b)
                *(long*)s = Value.KeyBlock;
            return b;
        }

        public override int[] GetKeyIds()
        {            
            return Collection.Rubrics.KeyRubrics.AsValues().Select(r => r.FigureFieldId).ToArray();
        }

        public override object[] GetKeyIdValues()
        {
            return Collection.Rubrics.KeyRubrics.AsValues().Select(r => Value[r.FigureFieldId]).ToArray();
        }

        public override long Key
        {
            get
            {
                return Value.KeyBlock;
            }
            set
            {
                Value.KeyBlock = value;
            }
        }

        public object[] ValueArray
        {
            get
            {
                if (Presets == null)
                    return Value.ValueArray;
                object[] valarr = Value.ValueArray;
                Presets.AsCards().Select((x) => valarr[x.Key] = x.Value).ToArray();
                return valarr;
            }
            set
            {
                for (int i = 0; i < value.Length; i++)
                    SetPreset(i, value[i]);
            }
        }

        public     Ussn SystemSerialCode
        {
            get => Value.SystemSerialCode;
            set => Value.SystemSerialCode = value;
        }
       
        public IFigures Collection;

        public Deck<object> Presets = null;

        public object GetPreset(int fieldId)
        {
            object val = null;
            if (Presets != null && Presets.TryGet(fieldId, out val))
                return val;
            return Value[fieldId];
        }
        public object GetPreset(string propertyName)
        {
            MemberRubric rubric = null;
            if (Collection.Rubrics.TryGet(propertyName, out rubric))
                return GetPreset(rubric.FigureFieldId);
            throw new IndexOutOfRangeException("Field doesn't exist");
        }

        public Card<object>[] GetPresets()
        {
            return Presets.AsCards().ToArray();
        }

        public void SetPreset(int fieldId, object value)
        {
            if (Presets == null)
                Presets = new Deck<object>(4);
            Presets.Put(fieldId, value);
        }
        public void SetPreset(string propertyName, object value)
        {
            MemberRubric rubric = null;
            if (Collection.Rubrics.TryGet(propertyName, out rubric))
                SetPreset(rubric.FigureFieldId, value);
            else
                throw new IndexOutOfRangeException("Field doesn't exist");
        }

        public void WritePresets()
        {
            foreach (var c in Presets)
                Value[(int)c.Key] = c.Value;
            Presets = null;
        }

        public bool HavePresets
          => Presets != null ?
          true :
          false;

    
    }
}
