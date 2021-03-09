using System;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class SubmissionResult<T> where T : IMember
    {
        public string Id { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Language { get; set; }
        public bool IsPassed { get; set; }
        public string State { get; set; }
        public bool IsReviewed { get; set; }
        public int ReviewResult { get; set; }
        public int? ExecutionDuration { get; set; }
        public bool HasReviewData { get; set; }
        public bool IsQueued { get; set; }
        public bool EnableReport { get; set; }
        public bool EnableRerun { get; set; }
        public T Member { get; set; }
        public bool NowFailing { get; set; }
    }
}
