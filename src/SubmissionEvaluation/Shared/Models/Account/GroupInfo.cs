namespace SubmissionEvaluation.Shared.Models.Account
{
    public class GroupInfo
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public bool Selected { get; set; }
        public bool IsSuperGroup { get; set; }
        public GroupInfo[] SubGroups { get; set; } = new GroupInfo[] { };
    }
}
