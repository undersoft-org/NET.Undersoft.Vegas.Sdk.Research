using System.Uniques;

namespace System.Labors
{
    public class Note : IUnique
    {
      
        public Note(string sender, params object[] Params) : this(sender, null, null, null, Params) { }
        public Note(string sender, string recipient, params object[] Params) : this(sender, recipient, null, null, Params) { }
        public Note(string sender, string recipient, NoteEvoker Out, params object[] Params) : this(sender, recipient, Out, null, Params) { }
        public Note(string sender, string recipient, NoteEvoker Out, NoteEvokers In, params object[] Params)
        {
            SenderName = sender;
            Parameters = Params;

            if (recipient != null)
                RecipientName = recipient;

            if (Out != null)
                EvokerOut = Out;

            if (In != null)
                EvokersIn = In;
        }

        public Note(Labor sender, Labor recipient, NoteEvoker Out, NoteEvokers In, params object[] Params)
        {
            Parameters = Params;

            if (recipient != null)
            {
                Recipient = recipient;
                RecipientName = Recipient.Laborer.LaborerName;
            }

            Sender = sender;
            SenderName = Sender.Laborer.LaborerName;

            if (Out != null)
                EvokerOut = Out;

            if (In != null)
                EvokersIn = In;
        }

        public NoteEvoker EvokerOut { get; set; }
        public NoteEvokers EvokersIn { get; set; }

        public NoteBox SenderBox;

        public string    RecipientName { get; set; }
        public Labor     Recipient { get; set; }

        public string    SenderName { get; set; }
        public Labor     Sender { get; set; }

        public object[] Parameters;

        #region IUnique

        public IUnique Empty => new Usid();

        public long KeyBlock { get => Sender.KeyBlock; set => Sender.KeyBlock = value; }

        public byte[] GetBytes()
        {
            return Sender.GetBytes();
        }
        public byte[] GetKeyBytes()
        {
            return Sender.GetKeyBytes();
        }
        public void SetHashKey(long value)
        {
            Sender.KeyBlock = value;
        }
        public long GetHashKey()
        {
            return Sender.GetHashKey();
        }
        public bool Equals(IUnique other)
        {
            return Sender.Equals(other);
        }
        public int CompareTo(IUnique other)
        {
            return Sender.CompareTo(other);
        }

        #endregion

    }
}
