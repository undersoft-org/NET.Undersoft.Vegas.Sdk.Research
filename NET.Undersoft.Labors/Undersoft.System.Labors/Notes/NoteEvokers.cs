using System.Linq;
using System.Uniques;
using System.Extract;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Multemic;

namespace System.Labors
{
    public class NoteEvokers : Catalog<NoteEvoker>
    {
        public void AddRange(List<NoteEvoker> _evokers)
        {
            foreach (NoteEvoker evoker in _evokers)
                Add(evoker);
        }
        public bool Have(List<Labor> objectives)
        {
            return this.AsValues().Where(t => t.RelationLabors.Where(ro => objectives.All(o => ReferenceEquals(ro, o))).Any()).Any();
        }
        public bool Have(List<string> relayNames)
        {
            return this.AsValues().Where(t => t.RelationNames.SequenceEqual(relayNames)).Any();
        }

        public NoteEvoker this[string RelationName]
        {
            get
            {
                return this.AsValues().Where(c => c.RelationNames.Contains(RelationName)).First();
            }
        }
        public NoteEvoker this[Labor objective]
        {
            get
            {
                return this.AsValues().Where(c => c.RelationLabors.Where(ro => ReferenceEquals(ro, objective)).Any()).First();
            }
        }
    }

}
