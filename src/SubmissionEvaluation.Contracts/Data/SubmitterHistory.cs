using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class SubmitterHistory
    {
        public List<HistoryEntry> Entries { get; set; }
        public string Id { get; set; }
    }
}
