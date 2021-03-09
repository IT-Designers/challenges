namespace SubmissionEvaluation.Contracts.Data
{
    public class ComparisonResult
    {
        public bool Success { get; set; }
        public string Difference { get; set; }
        public string Expected { get; set; }
        public string Output { get; set; }
        public bool Evaluated { get; set; }
    }
}
