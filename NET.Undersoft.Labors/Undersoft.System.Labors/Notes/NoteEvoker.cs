using System.Linq;
using System.Uniques;
using System.Extract;
using System.Collections.Generic;
using System.Multemic;

namespace System.Labors
{
    public class NoteEvoker : Board<Labor>, IUnique
    {     
        public NoteEvoker(Labor sender, string recipientName, IList<string> relayNames)
        {
            Sender = sender;
            SenderName = sender.Laborer.LaborerName;
            List<Labor> objvl = Sender.Scope.Subjects.AsCards().Where(m => m.Value.Labors.ContainsKey(recipientName)).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
            if (objvl.Any())
                Recipient = objvl.First();
            RecipientName = recipientName;
            SystemCode = new Usid(($"{SenderName}.{RecipientName}").GetHashKey());
            RelationNames = new List<string>(relayNames);
            RelationLabors = Sender.Scope.Subjects.AsCards().Where(m => m.Value.Labors.AsIdentifiers().Where(k => relayNames.Select(rn => rn.GetHashKey()).Contains(k.KeyBlock)).Any()).SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
        }
        public NoteEvoker(Labor sender, string recipientName, IList<Labor> relayLabors)
        {
            Sender = sender;
            SenderName = sender.Laborer.LaborerName;
            RecipientName = recipientName;
            SystemCode = new Usid(($"{SenderName}.{RecipientName}").GetHashKey());
            List<Labor> objvl = Sender.Scope.Subjects.AsCards()
                                        .Where(m => m.Value.Labors.ContainsKey(recipientName))
                                            .SelectMany(os => os.Value.Labors.AsCards().Select(o => o.Value)).ToList();
            if (objvl.Any())
                Recipient = objvl.First();
            RelationLabors = new List<Labor>(relayLabors);
            RelationNames.AddRange(RelationLabors.Select(rn => rn.Laborer.LaborerName));
        }
        public NoteEvoker(Labor sender, Labor recipient, List<Labor> relayLabors)
        {
            Sender = sender;
            SenderName = sender.Laborer.LaborerName;
            Recipient = recipient;
            RecipientName = recipient.Laborer.LaborerName;
            SystemCode = new Usid(($"{SenderName}.{RecipientName}").GetHashKey());
            RelationLabors = relayLabors;
            RelationNames.AddRange(RelationLabors.Select(rn => rn.Laborer.LaborerName));
        }
        public NoteEvoker(Labor sender, Labor recipient, List<string> relayNames)
        {
            Sender = sender;
            SenderName = sender.Laborer.LaborerName;
            Recipient = recipient;
            RecipientName = recipient.Laborer.LaborerName;
            SystemCode = new Usid(($"{SenderName}.{RecipientName}").GetHashKey());
            RelationNames = relayNames;
            RelationLabors = Sender.Scope.Subjects.AsCards()
                                    .Where(m => m.Value.Labors.AsIdentifiers()
                                        .Where(k => relayNames
                                            .Select(rn => rn.GetHashKey()).Contains(k.KeyBlock)).Any())
                                                .SelectMany(os => os.Value.Labors.AsCards()
                                                    .Select(o => o.Value)).ToList();
        }

        public string EvokerName { get; set; }

        public string RecipientName { get; set; }
        public Labor Recipient { get; set; }

        public string SenderName { get; set; }
        public Labor Sender { get; set; }

        public List<string> RelationNames = new List<string>();
        public List<Labor> RelationLabors = new List<Labor>();

        public EvokerType EvokerType { get; set; }

        #region IUnique

        private Usid SystemCode;

        public IUnique Empty => new Usid();

        public long KeyBlock { get => SystemCode.KeyBlock; set => SystemCode.KeyBlock = value; }

        public byte[] GetBytes()
        {
            return ($"{SenderName}.{RecipientName}").GetBytes();
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
