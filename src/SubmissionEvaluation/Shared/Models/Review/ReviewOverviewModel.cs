using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Review
{
    public class ReviewOverviewModel : GenericModel
    {
        public IEnumerable<Result> ReviewableSubmissions { get; set; }
        public IEnumerable<RunningReviewModel> RunningReviews { get; set; }
    }
}
