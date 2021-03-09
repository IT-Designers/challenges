namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class EditFileModel : GenericModel
    {
        public string ChallengeId { get; set; }
        public string RelativeFilePath { get; set; }
        public string FileContent { get; set; }
    }
}
