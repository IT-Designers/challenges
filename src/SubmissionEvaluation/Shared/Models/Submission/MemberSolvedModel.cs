using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class MemberSolvedModel<T> : ProfileHeaderModel where T : IChallenge
    {
        public string Id { get; set; }
        public Dictionary<string, SolvedInfoForChallenge> Solved { get; set; }
        public List<string> Compilers { get; set; }
        public IEnumerable<T> Challenges { get; set; }
    }
}
