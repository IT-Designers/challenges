namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewData
    {
        public string Challenge { get; set; }
        public string Id { get; set; }
        public string Time { get; set; }
        public ReviewCodeComments[] ResultComments { get; set; }
        public GuidedQuestion[] GuidedQuestionResult { get; set; }
        public ReviewCategoryResult[] CategoryResults { get; set; }
    }
}
