using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data.Ranklist
{
    public class SubmissionEntry
    {
        public int Rank { get; set; }
        public string Id { get; set; }
        public string Language { get; set; }
        public IEnumerable<string> MoreLanguages { get; set; }
        public int Exectime { get; set; }
        public DateTime Date { get; set; }
        public int Points { get; set; }
        public int SubmissionCount { get; set; }
        public int CustomScore { get; set; }
        public int Rating { get; set; }
        public int DuplicateScore { get; set; }
    }
}
