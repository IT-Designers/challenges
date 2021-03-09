using System;

namespace SubmissionEvaluation.Contracts.Data.Ranklist
{
    public class Semester
    {
        public SemesterPeriod Period { get; set; }
        public string Years { get; set; }
        public DateTime FirstDay { get; set; }
        public DateTime LastDay { get; set; }
    }
}
