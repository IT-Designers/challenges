namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewTemplate
    {
        public string Id { get; set; }
        public string ToolVersion { get; set; }
        public ReviewCategory[] Categories { get; set; }
        public GuidedQuestion[] GuidedQuestions { get; set; }
    }
}
