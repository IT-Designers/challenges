namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewCategoryResult
    {
        public string CategoryId { get; set; }
        public string CategoryComments { get; set; }
        public float? Grade { get; set; } //Is between 1 and 6 or null if no rating
    }
}
