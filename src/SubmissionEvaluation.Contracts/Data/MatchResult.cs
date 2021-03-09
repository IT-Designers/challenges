using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class MatchResult
    {
        public int Players { get; set; }
        public int Duration { get; set; }
        public int Turns { get; set; }
        public string MapName { get; set; }
        public List<string> ResultFiles { get; set; } = new List<string>();
        public List<int> Ranking { get; set; } = new List<int>();
        public List<int> MaxTimes { get; set; } = new List<int>();
        public bool Succeeded { get; set; }
        public string ErrorMessage { get; set; }
    }
}
