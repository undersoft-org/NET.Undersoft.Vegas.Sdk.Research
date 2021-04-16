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
    public class Laborator
    {
        readonly object holder = new object();
        readonly object holderIO = new object();

        private Thread[] laborers;
        private Board<Laborer> LaborersQueue = 
            new Board<Laborer>();

        public LaborNotes Notes;
        public Subject Subject;
        public Scope Scope;
        public bool Ready;
        private int LaborersCount;

        public Laborator(Subject mission)
        {
            Subject = mission;
            Scope = Subject.Scope;
            Notes = Scope.Notes;
            LaborersCount = Subject.LaborersCount;
            Ready = false;
        }

        public void CreateLaborers(int antcount = 1)
        {
            if (antcount > 1)
            {
                LaborersCount = antcount;
                Subject.LaborersCount = antcount;
            }
            else
                LaborersCount = Subject.LaborersCount;

            laborers = new Thread[LaborersCount];
            // Create and start a separate thread for each Ant
            for (int i = 0; i < LaborersCount; i++)
            {
                laborers[i] = new Thread(ActivateLaborer);
                laborers[i].Start();               
            }
        }
        public void Reset(int antcount = 1)
        {
            Close(true);
            CreateLaborers(antcount);
        }
        public void Elaborate(Laborer worker)
        {
            lock (holder)
            {
                if (worker != null)
                {
                    Laborer Worker = CloneLaborer(worker);
                    LaborersQueue.Enqueue(Worker);
                    Monitor.Pulse(holder);
                   
                }
                else
                {
                    LaborersQueue.Enqueue(DateTime.Now.ToBinary(), worker);
                    Monitor.Pulse(holder);
                }
            }
        }

        private void ActivateLaborer()
        {
            while (true)
            {
                Laborer worker = null;
                object input = null;
                lock (holder)
                {
                    do
                    {
                        while (LaborersQueue.Count == 0)
                        {
                            Monitor.Wait(holder);
                        }
                    } while (!LaborersQueue.TryDequeue(out worker));


                    if (worker != null)
                        input = worker.Input;
                }

                if (worker == null)
                    return;

                object output = null;

                if (input != null)
                {
                    if (input is IList)
                        output = worker.Work.Execute((object[])input);
                    else
                        output = worker.Work.Execute(input);
                }
                else
                    output = worker.Work.Execute();

                lock (holderIO)
                    Outpost(worker, output);

            }            
        }

        public void  Close(bool SafeClose)
        {
            // Enqueue one null item per worker to make each exit.
            foreach (Thread laborer in laborers)
            {             
                Elaborate(null);
                
                if (SafeClose && laborer.ThreadState == ThreadState.Running)
                    laborer.Join();               
            }                  
        }

        private Laborer CloneLaborer(Laborer laborer)
        {
            Laborer _laborer = new Laborer(laborer.LaborerName, laborer.Work);
            _laborer.Input = laborer.Input;
            _laborer.EvokersIn = laborer.EvokersIn;
            _laborer.Labor = laborer.Labor;
            return _laborer;
        }

        private void Outpost(Laborer worker, object output)
        {
            if (output != null)
            {
                worker.Output = output;

                if (worker.EvokersIn != null && worker.EvokersIn.AsValues().Any())
                {
                    List<Note> intios = new List<Note>();
                    foreach (NoteEvoker evoker in worker.EvokersIn.AsValues())
                    {
                        Note intio = new Note(worker.Labor, evoker.Recipient, evoker, null, output);
                        intio.SenderBox = worker.Labor.Box;
                        intios.Add(intio);
                    }

                    if (intios.Any())
                        Notes.Send(intios);
                }
               
            }
        }
    }

}
