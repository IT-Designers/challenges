using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Members
{
    public class MemberModel<T, S> : ProfileHeaderModel where T : ISubmission where S : IMember
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int GlobalScore { get; set; }
        public int SolvedCount { get; set; }
        public int TotalCount { get; set; }
        public Dictionary<string, Award> Achievements { get; set; }
        public List<HistoryEntry> History { get; set; }
        public List<PointsHoldTupel<T, S>> Points { get; set; }
    }
}
