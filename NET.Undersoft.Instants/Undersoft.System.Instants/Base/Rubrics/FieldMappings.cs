using System.Collections.Generic;
using System.Multemic;
using System.Uniques;

namespace System.Instants
{
    [Serializable]
    public class FieldMapping
    {
        public FieldMapping(string dbDeckName) : this(dbDeckName, new Deck<int>(), new Deck<int>()) { }                
        public FieldMapping(string dbDeckName, IDeck<int> keyOrdinal, IDeck<int> columnOrdinal)
        {
            KeyOrdinal = keyOrdinal;
            ColumnOrdinal = columnOrdinal;
            DbTableName = dbDeckName;
        }

        public string DbTableName { get; set; }

        public IDeck<int> KeyOrdinal { get; set; }
        public IDeck<int> ColumnOrdinal { get; set; }
    }

    [Serializable]
    public class FieldMappings : CardBook<FieldMapping>
    {
        public override Card<FieldMapping> EmptyCard()
        {
            return new Card64<FieldMapping>();
        }

        public override Card<FieldMapping>[] EmptyCardTable(int size)
        {
            return new Card64<FieldMapping>[size];
        }
        public override Card<FieldMapping>[] EmptyCardList(int size)
        {
            return new Card64<FieldMapping>[size];
        }

        public override Card<FieldMapping> NewCard(object key, FieldMapping value)
        {
            return new Card64<FieldMapping>(key, value);
        }

        public override Card<FieldMapping> NewCard(long key, FieldMapping value)
        {
            return new Card64<FieldMapping>(key, value);
        }

        public override Card<FieldMapping> NewCard(FieldMapping value)
        {
            return new Card64<FieldMapping>(value.DbTableName.GetHashKey(), value);
        }
        public override Card<FieldMapping> NewCard(Card<FieldMapping> value)
        {
            return new Card64<FieldMapping>(value);
        }
    }

}
