using System;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentStats
    {
        public string Name { get; set; }
        public DateTime LastRun { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title { get; set; }
    }
}
