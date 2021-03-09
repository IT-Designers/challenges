namespace SubmissionEvaluation.Shared.Models.Review
{
    public class CommentModel
    {
        public string File { get; set; }
        public string Id { get; set; }
        public string Issue { get; set; }
        public int Length { get; set; }
        public int Offset { get; set; }
        public string Text { get; set; }
    }
}
