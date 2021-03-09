namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class AdminModel : ProfileHeaderModel
    {
        public ManageUserModel ManageUser { get; set; } = new ManageUserModel();
        public AddTempUserModel AddTempUser { get; set; } = new AddTempUserModel();
    }
}
