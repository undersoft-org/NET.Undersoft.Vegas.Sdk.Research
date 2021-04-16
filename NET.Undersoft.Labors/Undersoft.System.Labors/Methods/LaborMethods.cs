using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Instants;
using System.Multemic;
using System.Uniques;
using System.Extract;
using System.Linq;

namespace System.Labors
{   

    public class LaborMethods : Catalog<IDeputy>
    {
        public override Card<IDeputy> EmptyCard()
        {
            return new LaborMethod();
        }

        public override Card<IDeputy>[] EmptyCardTable(int size)
        {
            return new LaborMethod[size];
        }
        public override Card<IDeputy>[] EmptyCardList(int size)
        {
            return new LaborMethod[size];
        }

        public override Card<IDeputy> NewCard(long key, IDeputy value)
        {
            return new LaborMethod(key, value);
        }

        public override Card<IDeputy> NewCard(object key, IDeputy value)
        {
            return new LaborMethod(key, value);
        }

        public override Card<IDeputy> NewCard(IDeputy value)
        {
            return new LaborMethod(value);
        }
    }
}
