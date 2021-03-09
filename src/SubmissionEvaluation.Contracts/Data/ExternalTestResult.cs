namespace SubmissionEvaluation.Contracts.Data
{
    public class ExternalTestResult
    {
        public bool IsPassed { get; set; }
        public string Diff { get; set; }
        public int? CustomScore { get; set; }
    }
}
