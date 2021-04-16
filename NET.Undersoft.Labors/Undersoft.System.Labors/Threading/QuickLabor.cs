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
    public class QuickLabor
    {
        public Laboratory Lab;

        public QuickLabor(bool safeClose, string className, string methodName, out object result, params object[] input) 
            : this(1, safeClose, Summon.New(className), methodName, input)
        {
            result = Laborer.Output;
        }
        public QuickLabor(string className, string methodName, params object[] input) 
            : this(1, false, Summon.New(className), methodName, input)
        {
        }       
        public QuickLabor(object classObject, string methodName, out object result, params object[] input)
            : this(1, false, classObject, methodName, input)
        {
            result = Laborer.Input;
        }
        public QuickLabor(int laborersCount, bool safeClose, object classObject, string methodName, params object[] input)
        {
            IDeputy am = new InstantDeputy(classObject, methodName);
            LaborMethods _ant = new LaborMethods();
            _ant.Put(am);
            Lab = new Laboratory(_ant);
            Lab.Scope["Primary"].LaborersCount = laborersCount;
            Lab.RunLaborators();           
            Subject = Lab.Scope["Primary"];           
            Visor = Subject.Visor;
            Laborer = Subject.Labors.AsValues().ElementAt(0).Laborer;
            Lab.Elaborate(am.Info.Name, input);
            Subject.Visor.Close(safeClose);
        }
        public QuickLabor(int laborersCount, bool safeClose, IDeck<IDeputy> _methods)
        {           
            LaborMethods _ant = new LaborMethods();
            foreach(var am in _methods)
                _ant.Put(am);
            Lab = new Laboratory(_ant);
            Lab.Scope["Primary"].LaborersCount = laborersCount;
            Lab.RunLaborators();
            Subject = Lab.Scope["Primary"];          
            Visor = Subject.Visor;
            foreach (var am in _methods)
                Lab.Elaborate(am.Info.Name, am.ParameterValues);
            Subject.Visor.Close(safeClose);
        }
        public QuickLabor(int laborersCount, int evokerCount, bool safeClose, IDeputy method, IDeputy evoker)
        {
            LaborMethods _ant = new LaborMethods();
            _ant.Put(method);
            _ant.Put(evoker);
            Lab = new Laboratory(_ant);
            Lab.Scope["Primary"].LaborersCount = laborersCount;
            Lab.RunLaborators();
            Subject = Lab.Scope["Primary"];
            Visor = Subject.Visor;
            Laborer = Subject.Labors.AsValues().ElementAt(0).Laborer;
            Laborer.AddEvoker(Subject.Labors.AsCards().Skip(1).First().Value);
            Lab.Elaborate(method.Info.Name, method.ParameterValues);
            Subject.Visor.Close(safeClose);
        }
       
        public void Close(bool safeClose = false)
        {
            Subject.Visor.Close(safeClose);
        }
        public void Elaborate()
        {
            Visor.Elaborate(this.Laborer);
        }
        public void Elaborate(params object[] input)
        {
            this.Laborer.Input = input;
            Visor.Elaborate(this.Laborer);
        }
        public Subject Subject;
        public Laborator Visor;
        public Laborer Laborer;


    }
}
