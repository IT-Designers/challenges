using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewCodeComments
    {
        public string FileName { get; set; }
        public List<ReviewComments> Comments { get; set; }
    }
}
