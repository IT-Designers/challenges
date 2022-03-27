using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;

namespace SubmissionEvaluation.Shared.Models
{
    public class MemberListModel<T> : GenericModel where T : IMember
    {
        public bool IsSemesterRanking { get; set; }
        public string CurrentSemester { get; set; }
        public Dictionary<string, T> Members { get; set; }
        public IEnumerable<GlobalSubmitter> Submitters { get; set; }
        public string FilterMode { get; set; }
        public string FilterValue { get; set; }
    }
}
