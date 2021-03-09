using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data.Ranklist
{
    public class GlobalRanklist
    {
        public List<GlobalSubmitter> Submitters { get; set; }
        public DateTime LastRankingChange { get; set; }
        public Semester CurrentSemester { get; set; }
    }
}
