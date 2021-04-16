using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Multemic;
using System.Uniques;
using System.Instants;

namespace System.Labors
{   
    public class Laborer : IUnique
    {            
        public Laborer()
        {
            input = new Board<object>();
            output = new Board<object>();
            EvokersIn = new NoteEvokers();          
        }
        public Laborer(string Name, IDeputy Method) : this()
        {
            Work = Method;
            LaborerName = Name;

            SystemCode = new Usid(($"{Work.KeyBlock}.{DateTime.Now.ToBinary()}").GetHashKey());

        }       

        public string LaborerName { get; set; }

        public IDeputy Work
        { get; set; }

        public Labor Labor
        { get; set; }

        private Board<object> input;
        public object Input
        {

            get
            {
                object _entry = null;
                input.TryDequeue(out _entry);
                return _entry;

            }
            set
            {
                input.Enqueue(value);
            }


        }

        private Board<object> output;
        public object Output
        {
            get
            {
                object _result = null;
                if (output.TryDequeue(out _result))
                    return _result;
                return null;
            }
            set
            {
                output.Enqueue(value);
            }

        }

        public void AddEvoker(string RecipientName, List<string> RelationNames)
        {
            EvokersIn.Add(new NoteEvoker(Labor, RecipientName, RelationNames));
        }
        public void AddEvoker(Labor Recipient, List<Labor> RelationLabors)
        {
            EvokersIn.Add(new NoteEvoker(Labor, Recipient, RelationLabors));
        }
        public void AddEvoker(string RecipientName)
        {
            EvokersIn.Add(new NoteEvoker(Labor, RecipientName, new List<string>() { LaborerName }));
        }
        public void AddEvoker(Labor Recipient)
        {
            EvokersIn.Add(new NoteEvoker(Labor, Recipient, new List<Labor>() { Labor }));
        }

        public NoteEvokers EvokersIn { get; set; }

        #region IUnique

        private Usid SystemCode;

        public IUnique Empty => new Usid();

        public long KeyBlock { get => SystemCode.KeyBlock; set => SystemCode.KeyBlock = value; }

        public byte[] GetBytes()
        {
            return SystemCode.GetBytes();
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
        
        #endregion
    }
}
