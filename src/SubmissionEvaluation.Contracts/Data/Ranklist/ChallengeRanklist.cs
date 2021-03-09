using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data.Ranklist
{
    public class ChallengeRanklist
    {
        public ChallengeRanklist()
        {
            Submitters = new List<SubmissionEntry>();
        }

        public string Challenge { get; set; }
        public List<SubmissionEntry> Submitters { get; set; }
        public Dictionary<string, int> SolvedCount { get; set; }
    }
}
