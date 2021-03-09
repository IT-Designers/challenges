namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewComments
    {
        public int Line { get; set; } //Not in use
        public string Text { get; set; } //Comment
        public string Id { get; set; } //CategoryId of comment Or Issue ID?
        public int Offset { get; set; } //Index in Source-string; not index in line
        public int Length { get; set; } //Length of marked Text
        public string AssignedIssue { get; set; }
    }
}
