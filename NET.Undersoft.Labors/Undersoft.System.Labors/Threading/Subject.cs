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
    public class Subject
    {
        public Scope Scope { get; set; }

        public Subject(string Name)
        {
            SubjectName = Name;
            LaborersCount = 1;           
        }
        public Subject(string Name, IList<Labor> LaborList)
        {
            SubjectName = Name;        
            foreach (Labor objective in LaborList)
            {
                objective.Scope = Scope;
                objective.Subject = this;
                Labors.Put(objective.Name, objective);
            }
            LaborersCount = 1;
        }
        public Subject(string Name, IList<IDeputy> MethodList)
        {
            SubjectName = Name;           
            foreach (IDeputy method in MethodList)
            {
                Labor objective = new Labor($"{method.Info.Name}", method);
                objective.Scope = Scope;
                objective.Subject = this;
                Labors.Put($"{method.Info.Name}", objective);
            }
            LaborersCount = 1;
        }

        public int LaborersCount { get; set; }

        public Laborator Visor { get; set; }

        public string SubjectName { get; set; }

        public Catalog<Labor> Labors
        { get; } = new Catalog<Labor>();

        public Labor Get(string key)
        {
            Labor result = null;
            Labors.TryGet(key, out result);
            return result;
        }
        public void Add(Labor value)
        {
            value.Scope = Scope;
            value.Subject = this;
            Labors.Put(value.Laborer.LaborerName, value);
        }
        public void Add(IDeputy value)
        {
            Labor obj = new Labor($"{value.Info.Name}", value);
            obj.Scope = Scope;
            obj.Subject = this;
            Labors.Put(obj.Laborer.LaborerName, obj);
        }            
        public void AddRange(IList<Labor> value)
        {
            foreach (Labor obj in value)
            {
                obj.Scope = Scope;
                obj.Subject = this;
                Labors.Put($"{obj.Info.Name}", obj);
            }
        }
        public void AddRange(IList<IDeputy> value)
        {
            foreach (IDeputy obj in value)
            {
                Labor oj = new Labor($"{obj.Info.Name}", obj);
                oj.Scope = Scope;
                oj.Subject = this;
                Labors.Put($"{obj.Info.Name}", oj);
            }
        }
      
        public Labor this[string key]
        {
            get
            {
                Labor result = null;
                Labors.TryGet(key, out result);
                return result;
            }
            set
            {
                value.Scope = Scope;
                value.Subject = this;
                Labors.Put(key, value);
            }
        }       
    }
}
