using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class SubmitterRankings
    {
        public SubmitterRankings()
        {
            Submissions = new List<SubmitterRankingEntry>();
        }

        public List<SubmitterRankingEntry> Submissions { get; set; }
        public string Name { get; set; }
    }
}
