using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Linq;
using System.Multemic;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Globalization;

namespace System.Instants
{
    public partial class MemberRubrics : CardBook<MemberRubric>
    {
        public MemberRubrics() 
            : base() { }
        public MemberRubrics(IList<MemberRubric> collection) 
            : base(collection) { }
        public MemberRubrics(IEnumerable<MemberRubric> collection) 
            : base(collection) { }

        public override Card<MemberRubric> EmptyCard()
        {
            return new RubricCard();
        }

        public override Card<MemberRubric>[] EmptyCardTable(int size)
        {
            return new RubricCard[size];
        }
        public override Card<MemberRubric>[] EmptyCardList(int size)
        {          
            return new RubricCard[size];
        }

        public override Card<MemberRubric> NewCard(object key, MemberRubric value)
        {
            return new RubricCard(key, value);
        }

        public override Card<MemberRubric> NewCard(long key, MemberRubric value)
        {
            return new RubricCard(key, value);
        }

        public override Card<MemberRubric> NewCard(MemberRubric value)
        {
            return new RubricCard(value.GetHashKey(), value);
        }
        public override Card<MemberRubric> NewCard(Card<MemberRubric> value)
        {
            return new RubricCard(value);
        }

        public MemberRubrics KeyRubrics { get; set; }

        public IFigures Collection { get; set; }

        public FieldMappings Mappings { get; set; }
    }


}
