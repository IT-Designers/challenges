using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class ReviewerModel : GenericModel
    {
        public Dictionary<string, ReviewLevelAndCounter> ReviewLanguages { get; set;}
    }
}
