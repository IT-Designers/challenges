using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class ManageUserModel
    {
        [Required(ErrorMessage = "Name muss angegeben werden")]
        public string Name { get; set; }

        public IMember Member { get; set; }
        public List<IMember> PendingMembers { get; set; }
        public string NewPassword { get; set; }
    }
}
