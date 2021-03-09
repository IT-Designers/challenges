using System;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentRankingEntry
    {
        public string MemberId { get; set; }
        public int Rank { get; set; }
        public int Elo { get; set; }
        public int Matches { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Loss { get; set; }
        public string Language { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsWorking { get; set; }
    }
}
