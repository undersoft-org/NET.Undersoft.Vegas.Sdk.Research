using System.Uniques;
using System.Multemic;
using System.Instants.Treatment;
using System.IO;

namespace System.Instants
{
    public abstract class FigureAlbum : FigureBook, IFigures
    {
        public abstract object this[int index, string propertyName] { get; set; }

        public abstract object this[int index, int fieldId] { get; set; }

        public abstract MemberRubrics Rubrics { get; set; }

        public abstract IFigure NewFigure();

        public abstract  Type FigureType { get; set; }

        public abstract int FigureSize { get; set; }

        public abstract Ussn SystemSerialCode { get; set; }

        public int Length => base.Cards.Length;

        public override Card<IFigure> EmptyCard()
        {
            var c = new FigureCard();
            c.Collection = this;
            return c;
        }

        public override Card<IFigure> NewCard(long key, IFigure value)
        {
            var c = new FigureCard(key, value);
            c.Collection = this;
            return c;
        }
        public override Card<IFigure> NewCard(object key, IFigure value)
        {
            var c = new FigureCard(key, value);
            c.Collection = this;
            return c;
        }

        public override Card<IFigure> NewCard(IFigure value)
        {
            var c = new FigureCard(value);
            c.Collection = this;
            return c;
        }

        public object[] ValueArray { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Type FiguresType { get; set; }

        public IFigures Exposition { get; set; }

        public FigureFilter Filter { get; set; }

        public FigureSort Sort { get; set; }

        public Func<IFigure, bool> Query { get; set; }

        public IUnique Empty => Ussn.Empty;

        public long KeyBlock { get => SystemSerialCode.KeyBlock; set => SystemSerialCode.SetHashKey(value); }

        object IFigure.this[int fieldId] { get => this[fieldId]; set => this[fieldId] = (IFigure)value; }
        public object this[string propertyName] { get => this[propertyName]; set => this[propertyName] = (IFigure)value; }

        public byte[] GetBytes()
        {
            return SystemSerialCode.GetBytes();
        }

        public byte[] GetKeyBytes()
        {
            return SystemSerialCode.GetKeyBytes();
        }

        public void SetHashKey(long value)
        {
            SystemSerialCode.SetHashKey(value);
        }

        public long GetHashKey()
        {
            return SystemSerialCode.GetHashKey();
        }

        public bool Equals(IUnique other)
        {
            return SystemSerialCode.Equals(other);
        }

        public int CompareTo(IUnique other)
        {
            return SystemSerialCode.CompareTo(other);
        }       

        #region Formatter

        public int SerialCount { get; set; }
        public int DeserialCount { get; set; }
        public int ProgressCount { get; set; }

        public int Serialize(Stream stream, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }
        public int Serialize(IFigurePacket buffor, int offset, int batchSize, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(Stream stream, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }
        public object Deserialize(ref object block, FigureFormat serialFormat = FigureFormat.Binary)
        {
            throw new NotImplementedException();
        }

        public object[] GetMessage()
        {
            return new[] { (IFigures)this };
        }

        public object GetHeader()
        {
            return this;
        }

        public int ItemsCount => throw new NotImplementedException();

        #endregion

    }
}