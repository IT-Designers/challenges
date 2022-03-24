using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.ClientPocos
{
    public class GroupMember
    {
        public GroupMember()
        {
        }

        public GroupMember(IMember member, int solvedCount, int points, int minDuplicate, int maxDuplicate, double averageDuplicate)
        {
            Id = member.Id;
            Uid = member.Uid;
            Name = member.Name;
            Mail = member.Mail;
            SolvedCount = solvedCount;
            Points = points;
            MinDuplicate = minDuplicate;
            MaxDuplicate = maxDuplicate;
            AverageDuplicate = averageDuplicate;
        }

        public string Id { get; set; }
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }
        public int SolvedCount { get; set; }
        public int Points { get; set; }
        public int MinDuplicate { get; set; }
        public int MaxDuplicate { get; set; }
        public double AverageDuplicate { get; set; }
    }
}
