using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentMatchResult
    {
        public int Id { get; set; }
        public string Map { get; set; }
        public DateTime Date { get; set; }
        public List<MatchResultPlayer> Players { get; set; } = new List<MatchResultPlayer>();
    }
}
