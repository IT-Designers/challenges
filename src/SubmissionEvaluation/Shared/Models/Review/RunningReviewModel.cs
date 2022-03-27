using System;

namespace SubmissionEvaluation.Shared.Models.Review
{
    public class RunningReviewModel
    {
        public string Challenge { get; set; }
        public string Submission { get; set; }
        public string ReviewerName { get; set; }
        public string Language { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
