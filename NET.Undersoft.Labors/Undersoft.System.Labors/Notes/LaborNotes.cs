using System.Linq;
using System.Collections.Generic;
using System.Multemic;

namespace System.Labors
{
    public class LaborNotes : Catalog<NoteBox>
    {

        private Scope Scope { get; set; }

        public void AddRecipient(string key, NoteBox noteBox)
        {
            if (noteBox != null)
            {
                if (noteBox.Labor != null)
                {
                    Labor objv = noteBox.Labor;
                    Put(noteBox.RecipientName, noteBox);
                }
                else
                {
                    List<Labor> objvl = Scope.Subjects.AsCards().Where(m => m.Value.Labors.ContainsKey(key)).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
                    if (objvl.Any())
                    {
                        Labor objv = objvl.First();
                        noteBox.Labor = objv;
                        Put(key, noteBox);
                    }
                }
            }
            else
            {
                List<Labor> objvl = Scope.Subjects.AsCards().Where(m => m.Value.Labors.ContainsKey(key)).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
                if (objvl.Any())
                {
                    Labor objv = objvl.First();
                    NoteBox iobox = new NoteBox(objv.Laborer.LaborerName);
                    iobox.Labor = objv;
                    Put(key, iobox);
                }
            }
        }
        public void SetRecipient(NoteBox value)
        {
            if (value != null)
            {
                if (value.Labor != null)
                {
                    Labor objv = value.Labor;
                    Put(value.RecipientName, value);
                }
                else
                {
                    List<Labor> objvl = Scope.Subjects.AsCards().Where(m => m.Value.Labors.ContainsKey(value.RecipientName)).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
                    if (objvl.Any())
                    {
                        Labor objv = objvl.First();
                        value.Labor = objv;
                        Put(value.RecipientName, value);
                    }
                }
            }
        }

        public void Send(Note parameters)
        {
            if (parameters.RecipientName != null && parameters.SenderName != null)
            {
                if (ContainsKey(parameters.RecipientName))
                {
                    NoteBox iobox = Get(parameters.RecipientName);
                    if (iobox != null)
                        iobox.AddNote(parameters);
                }
                else if (parameters.Recipient != null)
                {
                    Labor objv = parameters.Recipient;
                    NoteBox iobox = new NoteBox(objv.Laborer.LaborerName);
                    iobox.Labor = objv;
                    iobox.AddNote(parameters);
                    SetRecipient(iobox);
                }
                else if (Scope != null)
                {
                    List<Labor> objvl = Scope.Subjects.AsCards().Where(m => m.Value.Labors.ContainsKey(parameters.RecipientName)).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
                    if (objvl.Any())
                    {
                        Labor objv = objvl.First();
                        NoteBox iobox = new NoteBox(objv.Laborer.LaborerName);
                        iobox.Labor = objv;
                        iobox.AddNote(parameters);
                        SetRecipient(iobox);
                    }
                }
            }

        }
        public void Send(IList<Note> parametersList)
        {
            foreach (Note parameters in parametersList)
            {
                if (parameters.RecipientName != null && parameters.SenderName != null)
                {
                    if (ContainsKey(parameters.RecipientName))
                    {
                        NoteBox iobox = Get(parameters.RecipientName);
                        if (iobox != null)
                            iobox.AddNote(parameters);
                    }
                    else if (parameters.Recipient != null)
                    {
                        Labor objv = parameters.Recipient;
                        NoteBox iobox = new NoteBox(objv.Laborer.LaborerName);
                        iobox.Labor = objv;
                        iobox.AddNote(parameters);
                        SetRecipient(iobox);
                    }
                    else if (Scope != null)
                    {
                        List<Labor> objvl = Scope.Subjects.AsCards()
                                                .Where(m => m.Value.Labors.ContainsKey(parameters.RecipientName))
                                                    .SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
                        if (objvl.Any())
                        {
                            Labor objv = objvl.First();
                            NoteBox iobox = new NoteBox(objv.Laborer.LaborerName);
                            iobox.Labor = objv;
                            iobox.AddNote(parameters);
                            SetRecipient(iobox);
                        }
                    }
                }
            }

        }

    }

    public enum EvokerType
    {
        Always,
        Single,
        Schedule,
        Nome
    }

}
