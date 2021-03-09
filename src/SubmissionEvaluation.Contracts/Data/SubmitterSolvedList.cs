using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class SubmitterSolvedList
    {
        public Dictionary<string, SolvedInfoForChallenge> Solved { get; set; }
        public string Id { get; set; }
    }
}
