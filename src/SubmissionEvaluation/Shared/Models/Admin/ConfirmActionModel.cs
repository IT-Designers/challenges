namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class ConfirmActionModel : GenericModel
    {
        public string Id { get; set; }
        public string ActionMessage { get; set; }
        public string Action { get; set; }
    }

    public class ConfirmChallengeActionModel : GenericModel
    {
        public string Challenge { get; set; }
        public string ActivityMessage { get; set; }
        public string Activity { get; set; }
    }
}
