namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class Issue
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public int IssueCount { get; set; }
        public string Bad { get; set; }
        public string Enough { get; set; }
        public string SmellComment { get; set; }
        public int Quantifier { get; set; }
    }
}
