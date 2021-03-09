using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Permissions
{
    public class Permissions
    {
        public bool isAdmin { get; set; }
        public List<string> ViewPermissions { get; set; } = new List<string>();
        public List<string> CreatePermissions { get; set; } = new List<string>();
        public List<string> EditPermissions { get; set; } = new List<string>();
        public List<string> GroupsAccessible { get; set; } = new List<string>();
        public List<string> ChallengesAccessible { get; set; } = new List<string>();
        public List<string> BundlesAccessible { get; set; } = new List<string>();
        public List<string> MembersAccessible { get; set; } = new List<string>();
    }
}
