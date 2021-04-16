using System.Linq;
using System.Uniques;
using System.Multemic;

namespace System.Instants.Mathline
{
    public class MathRubrics : CardBook<MathRubric>
    {
        public int RubricsCount
        {
            get
            {
                return Rubrics.Count;
            }
        }
        public int RowsCount
        {
            get
            {
                return Data.Count;
            }
        }

        public IMultemic Data { get; set; }

        public MathRubrics(IMultemic data)
        {
            Rubrics = data.Rubrics;
            FormulaRubrics = new MathRubrics(Rubrics);
            MathlineRubrics = new MathRubrics(Rubrics);
            Data = data;
        }
        public MathRubrics(MemberRubrics rubrics)
        {
            Rubrics = rubrics;
            Data = rubrics.Collection;
        }
        public MathRubrics(MathRubrics rubrics)
        {
            Rubrics = rubrics.Rubrics;
            Data = rubrics.Data;
        }

        public MemberRubrics Rubrics
        { get; set; }

        public MathRubrics MathlineRubrics
        { get; set; } 
        public MathRubrics FormulaRubrics
        { get; set; } 

        public bool Combine(IMultemic table)
        {
            if (!ReferenceEquals(Data, table))
            {
                Data = table;
                CombinedReckoner[] evs = CombineReckoners();
                bool[] b = evs.Select(e => e.SetParams(Data, 0)).ToArray();
                return true;
            }
            CombineReckoners();
            return false;
        }
        public bool Combine()
        {
            if (!ReferenceEquals(Data, null))
            {
                CombinedReckoner[] evs = CombineReckoners();
                bool[] b = evs.Select(e => e.SetParams(Data, 0)).ToArray();
                return true;
            }
            CombineReckoners();
            return false;

        }

        public CombinedReckoner[] CombineReckoners()
        {
            return this.AsValues().Select(m => m.CombineReckoner()).ToArray();
        }

        public override Card<MathRubric> EmptyCard()
        {
            return new MathRubricCard();
        }
        public override Card<MathRubric>[] EmptyCardTable(int size)
        {
            return new MathRubricCard[size];
        }
        public override Card<MathRubric>[] EmptyCardList(int size)
        {
            return new MathRubricCard[size];
        }

        public override Card<MathRubric> NewCard(object key, MathRubric value)
        {
            return new MathRubricCard(key, value);
        }
        public override Card<MathRubric> NewCard(long key, MathRubric value)
        {
            return new MathRubricCard(key, value);
        }
        public override Card<MathRubric> NewCard(Card<MathRubric> value)
        {
            return new MathRubricCard(value);
        }
        public override Card<MathRubric> NewCard(MathRubric value)
        {
            return new MathRubricCard(value.GetHashKey(), value);
        }

    }


}
