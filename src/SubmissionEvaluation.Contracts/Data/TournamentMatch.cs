using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentMatch
    {
        public List<TournamentMatchEntry> Entries { get; set; } = new List<TournamentMatchEntry>();
    }
}
