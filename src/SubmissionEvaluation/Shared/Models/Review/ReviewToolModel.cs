using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Shared.Models.Review
{
    public class ReviewToolModel : GenericModel
    {
        public string Challenge { get; set; }
        public string SubmissionId { get; set; }
        public dynamic[] FileModel { get; set; }
        public GuidedQuestion[] GuidedQuestions { get; set; }
        public bool IsAdmin { get; set; }
        public string Langugage { get; set; }
        public List<(string Id, string Title)> Categories { get; set; }
        public List<ReviewFile> SourceFiles { get; set; }
    }
}
