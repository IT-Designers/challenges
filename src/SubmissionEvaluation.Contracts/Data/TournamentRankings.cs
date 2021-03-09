using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentRankings
    {
        public TournamentRankings()
        {
            Entries = new List<TournamentRankingEntry>();
        }

        public List<TournamentRankingEntry> Entries { get; set; }
        public string Name { get; set; }
    }
}
