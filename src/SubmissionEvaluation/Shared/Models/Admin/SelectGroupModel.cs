using SubmissionEvaluation.Shared.Models.Account;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class ManageMemberGroupsModel : GenericModel
    {
        public GroupInfo[] Groups { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class ManageMemberRolesModel : GenericModel
    {
        public GroupInfo[] Roles { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
