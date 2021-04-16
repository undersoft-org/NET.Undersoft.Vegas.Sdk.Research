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
    public class Labor : Task<object>, IDeputy
    {
        private Usid SystemCode;

        public IUnique Empty => new Usid();

        public long KeyBlock { get => SystemCode.KeyBlock; set => SystemCode.KeyBlock = value; }


        public Labor(string name, IDeputy method) : base(() => method.Execute())
        {
            Name = name;
            Laborer = new Laborer(name, method);
            Laborer.Labor = this;
            Box = new NoteBox(Laborer.LaborerName);
            Box.Labor = this;

            SystemCode = new Usid(method.GetHashKey());
        }   
        public Labor(Laborer laborer) : base(() => laborer.Work.Execute())
        {
            Name = laborer.LaborerName;
            Laborer = laborer;
            Laborer.Labor = this;
            Box = new NoteBox(Laborer.LaborerName);
            Box.Labor = this;

            SystemCode = new Usid(laborer.Work.GetHashKey());
        }

        public string Name { get; set; }

        public Laborer Laborer { get; set; }       

        public Subject Subject { get; set; }

        public Scope Scope { get; set; }

        public NoteBox Box { get; set; }

        public object[] ParameterValues
        {
            get => Laborer.Work.ParameterValues;
            set => Laborer.Work.ParameterValues = value;
        }

        public MethodInfo Info
        {
            get => Laborer.Work.Info;
            set => Laborer.Work.Info = value;
        }

        public ParameterInfo[] Parameters
        {
            get => Laborer.Work.Parameters;
            set => Laborer.Work.Parameters = value;
        }

        public void Elaborate(params object[] input)
        {
            Laborer.Input = input;
            this.Subject.Visor.Elaborate(Laborer);
        }

        public object Execute(params object[] parameters)
        {
            this.Elaborate(parameters);
            return null;
        }

        public byte[] GetBytes()
        {
            return Laborer.Work.GetBytes();
        }
        public byte[] GetKeyBytes()
        {
            return SystemCode.GetKeyBytes();
        }
        public void SetHashKey(long value)
        {
            SystemCode.KeyBlock = value;
        }
        public long GetHashKey()
        {
            return SystemCode.GetHashKey();
        }
        public bool Equals(IUnique other)
        {
            return SystemCode.Equals(other);
        }
        public int CompareTo(IUnique other)
        {
            return SystemCode.CompareTo(other);
        }
    }   
}
