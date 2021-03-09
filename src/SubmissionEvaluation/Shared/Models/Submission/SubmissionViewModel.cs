namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class SubmissionViewModel : GenericModel
    {
        public string ChallengeId { get; set; }
        public string[] SubmissionFilePaths { get; set; }
        public SourceCodeFile CurrentFile { get; set; }
    }
}
