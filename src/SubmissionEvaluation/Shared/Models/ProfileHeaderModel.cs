using System;

namespace SubmissionEvaluation.Shared.Models
{
    public class ProfileHeaderModel : GenericModel
    {
        public DateTime DateOfEntry { get; set; }
        public DateTime LastActivity { get; set; }
        public bool Inactive { get; set; }
        public int TotalPoints { get; set; }
        public int SolvedChallenges { get; set; }
        public int TotalChallenges { get; set; }
        public ProfileMenuType CurrentMenu { get; set; }
        public int Stars { get; set; }
    }
}
