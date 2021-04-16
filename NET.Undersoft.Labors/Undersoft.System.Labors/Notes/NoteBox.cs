using System.Linq;
using System.Collections.Generic;
using System.Multemic;

namespace System.Labors
{
    public class NoteBox : Board<NoteTopic>
    {
        public NoteBox(string Recipient)
        {
            RecipientName = Recipient;
            Evokers = new NoteEvokers();
        }

        public string RecipientName { get; set; }

        public Labor Labor { get; set; }

        public object[] GetParams(string key)
        {
            NoteTopic _ioqueue = null;
            Note temp = null;
            if (TryGet(key, out _ioqueue))
                if (_ioqueue.TryDequeue(out temp))
                    return temp.Parameters;
            return null;
        }
        public Note GetNote(string key)
        {
            NoteTopic _ioqueue = null;
            if (TryGet(key, out _ioqueue))
                return _ioqueue.Dequeue();
            return null;
        }

        public void AddNote(Note value)
        {
            if (value.SenderName != null)
            {
                NoteTopic queue = null;
                if (!ContainsKey(value.SenderName))
                {
                    queue = new NoteTopic(value.SenderName, this);
                    if (Add(value.SenderName, queue))
                    {
                        if (value.EvokerOut != null)
                            Evokers.Add(value.EvokerOut);
                        queue.AddNote(value);
                    }
                }
                else if (TryGet(value.SenderName, out queue))
                {
                    if (value.EvokerOut != null)
                        Evokers.Add(value.EvokerOut);
                    queue.AddNote(value);
                }
            }
        }
        public void AddNote(IList<Note> value)
        {
            if (value != null && value.Any())
            {
                foreach (Note antio in value)
                {
                    NoteTopic queue = null;
                    if (antio.SenderName != null)
                    {
                        if (!ContainsKey(antio.SenderName))
                        {
                            queue = new NoteTopic(antio.SenderName, this);
                            if (Add(antio.SenderName, queue))
                            {
                                if (antio.EvokerOut != null)
                                    Evokers.Add(antio.EvokerOut);
                                queue.AddNote(antio);
                            }
                        }
                        else if (TryGet(antio.SenderName, out queue))
                        {
                            if (value != null && value.Count > 0)
                            {
                                if (antio.EvokerOut != null)
                                    Evokers.Add(antio.EvokerOut);
                                queue.AddNote(antio);
                            }
                        }
                    }
                }
            }
        }       
        public void AddNote(string key, Note value)
        {
            value.SenderName = key;
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue))
                {
                    if (value.EvokerOut != null)
                        Evokers.Add(value.EvokerOut);
                    queue.AddNote(value);
                }
            }
            else if (TryGet(key, out queue))
            {
                if (value.EvokerOut != null)
                    Evokers.Add(value.EvokerOut);
                queue.AddNote(value);
            }
        }
        public void AddNote(string key, List<Note> value)
        {
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue) && value != null && value.Count > 0)
                {
                    foreach (Note notes in value)
                    {
                        if (notes.EvokerOut != null)
                            Evokers.Add(notes.EvokerOut);
                        notes.SenderName = key;
                        queue.AddNote(notes);
                    }
                }
            }
            else if (TryGet(key, out queue))
            {
                if (value != null && value.Count > 0)
                {
                    foreach (Note notes in value)
                    {
                        if (notes.EvokerOut != null)
                            Evokers.Add(notes.EvokerOut);
                        notes.SenderName = key;
                        queue.AddNote(notes);
                    }
                }
            }
        }
        public void AddNote(string key, object ioqueues)
        {
            NoteTopic queue = null;
            if (!ContainsKey(key))
            {
                queue = new NoteTopic(key, this);
                if (Add(key, queue) && ioqueues != null)
                {
                    queue.AddNote(ioqueues);
                }
            }
            else if (TryGet(key, out queue))
            {
                if (ioqueues != null)
                {
                    queue.AddNote(ioqueues);
                }
            }
        }
        public List<Note> TakeOut(List<string> keys)
        {
            List<Note> antios = this.AsCards().Where(q => keys.Contains(q.Value.SenderName)).Select(v => v.Value.Notes).ToList();
            return antios;
        }
        public bool MeetsRequirements(List<string> keys)
        {
            return this.AsCards().Where(q => keys.Contains(q.Value.SenderName)).All(v => v.Value.Count > 0);         
        }

        public NoteEvokers Evokers { get; set; }

        public void QualifyToEvoke()
        {
            List<NoteEvoker> toEvoke = new List<NoteEvoker>();
            foreach (NoteEvoker relay in Evokers.AsValues())
            {
                if (relay.RelationNames.All(r => ContainsKey(r)))
                    if (relay.RelationNames.All(r => this[r].AsValues().Any()))
                    {
                        toEvoke.Add(relay);
                    }
            }

            if (toEvoke.Any())
            {
                foreach (NoteEvoker evoke in toEvoke)
                {
                    if (MeetsRequirements(evoke.RelationNames))
                    {
                        List<Note> antios = TakeOut(evoke.RelationNames);

                        if (antios.All(a => a != null))
                        {
                            object[] parameters = new object[0];
                            object begin = Labor.Laborer.Input;
                            if (begin != null)
                                parameters = parameters.Concat((object[])begin).ToArray();
                            foreach (Note antio in antios)
                            {
                                if (antio.Parameters.GetType().IsArray)
                                    parameters = parameters.Concat(antio.Parameters.SelectMany(a => (object[])a).ToArray()).ToArray();
                                else
                                    parameters = parameters.Concat(antio.Parameters).ToArray();
                            }

                            Labor.Execute(parameters);
                        }
                    }
                }
            }
        }
    }
}
