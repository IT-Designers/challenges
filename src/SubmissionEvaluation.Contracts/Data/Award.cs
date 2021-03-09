using System;

namespace SubmissionEvaluation.Contracts.Data
{
    [Serializable]
    public class Award : IEquatable<Award>
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }

        public bool Equals(Award other)
        {
            return other != null && other.Id == Id;
        }

        public override bool Equals(object other)
        {
            return other is Award award && Equals(award);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
