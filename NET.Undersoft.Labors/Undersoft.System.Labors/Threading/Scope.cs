using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Extract;
using System.Instants;
using System.Multemic;
using System.Uniques;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace System.Labors
{
    public class Scope
    {
        public Scope(string name = null, LaborNotes notes = null)
        {
            ScopeName = (name != null) ? name : "Laboratory";
            Notes = (Notes != null) ? notes : new LaborNotes();
        }

        public string ScopeName { get; set; }

        public LaborNotes Notes { get; set; }

        public Subject Get(string key)
        {
            Subject result = null;
            Subjects.TryGet(key, out result);
            return result;
        }
        public void    Add(Subject value)
        {
            value.Scope = this;
            value.Visor = new Laborator(value);
            Subjects.Put(value.SubjectName, value);
        }
        public void    Add(string key, Subject value)
        {
            value.Scope = this;
            value.Visor = new Laborator(value);
            Subjects.Put(key, value);
        }
        public void    Add(string key, IList<Labor> value)
        {
            Subject msn = new Subject(key, value);
            msn.Scope = this;
            msn.Visor = new Laborator(msn);
            Subjects.Put(key, msn);
        }
        public Subject Add(string key, IList<IDeputy> value)
        {
            Subject msn = new Subject(key, value);
            msn.Scope = this;
            msn.Visor = new Laborator(msn);
            Subjects.Put(key, msn);
            return msn;
        }
        public void    Add(string key, IDeputy value)
        {
            List<IDeputy> cml = new List<IDeputy>() { value };
            Subject msn = new Subject(key, cml);
            msn.Scope = this;
            Subjects.Put(key, msn);
        }

        public Catalog<Subject> Subjects
        { get; } = new Catalog<Subject>();
        public Subject this[string key]
        {
            get
            {
                Subject result = null;
                Subjects.TryGet(key, out result);
                return result;
            }
            set
            {
                value.Scope = this;
                value.Visor = new Laborator(value);
                Subjects.Put(key, value);
            }
        }
    }
}
