namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class JsonUpload
    {
        public string ChallengeId { get; set; }
        public JsonFile[] Files { get; set; }
    }
}
