using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentMatches
    {
        public string Name { get; set; }
        public List<TournamentMatchResult> Results { get; set; }
    }
}
