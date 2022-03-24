using System.Collections.Generic;
using SubmissionEvaluation.Contracts.ClientPocos;

namespace SubmissionEvaluation.Shared.Models
{
    public class GroupMembers
    {
        public string GroupName { get; set; }
        public int? RequiredPoints { get; set; }
        public List<GroupMember> Members { get; set; } = new List<GroupMember>();
    }
}
