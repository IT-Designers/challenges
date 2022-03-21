using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SubmissionEvaluation.Contracts.Data;
using Group = SubmissionEvaluation.Contracts.ClientPocos.Group;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Shared.Models.Account
{
    public class SettingsModel : ProfileHeaderModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Du musst einen Namen angeben!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Du musst eine UserId angeben!")]
        public string Uid { get; set; }

        [EmailAddress] public string Mail { get; set; }

        public Dictionary<string, ReviewLevelAndCounter> ReviewLanguages { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public Dictionary<string, Award> Achievements { get; set; } = new Dictionary<string, Award>();
        public List<Group> Groups { get; set; } = new List<Group>();
        public bool CanChooseGroup { get; set; }
    }
}
