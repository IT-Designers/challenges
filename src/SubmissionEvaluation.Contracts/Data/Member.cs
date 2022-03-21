using System;
using System.Linq;
using System.Text.Json.Serialization;
using SubmissionEvaluation.Contracts.Data.Review;
using YamlDotNet.Serialization;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class Member : IMember
    {


        public static readonly string RemovedEntryId = "MEMBER_REMOVED";
        private string[] canRate;
        private string[] groups;
        private Dictionary<string, ReviewLevelAndCounter> reviewLanguages;

        private string[] roles;
        private string[] solvedChallenges;
        private string[] unlockedChallenges;

        public string Name { get; set; }
        public string Mail { get; set; }
        public string Id { get; set; }
        public string Uid { get; set; }
        public DateTime DateOfEntry { get; set; } = DateTime.Now;

        [YamlIgnore]
        [JsonIgnore]
        public bool IsReviewer
        {
            get {
                if (roles == null) return false;
                else return roles.Any("reviewer".Contains);
            }
        }

        /*[YamlIgnore]
        [JsonIgnore]
        public bool IsReviewerForLanguage(string language) =>
        */

        public int ReviewFrequency { get; set; } = 14;
        public DateTime LastReview { get; set; }

        public Dictionary<string, ReviewLevelAndCounter> ReviewLanguages { get => reviewLanguages ?? null; set => reviewLanguages = value; }

        public string[] Roles { get => roles ?? new string[0]; set => roles = value; }

        public string[] Groups { get => groups ?? new string[0]; set => groups = value; }

        public string[] UnlockedChallenges { get => unlockedChallenges ?? new string[0]; set => unlockedChallenges = value; }

        public string[] SolvedChallenges { get => solvedChallenges ?? new string[0]; set => solvedChallenges = value; }

        public string[] CanRate { get => canRate ?? new string[0]; set => canRate = value; }

        public string Password { get; set; }
        public DateTime? LastNotificationCheck { get; set; }

        [YamlIgnore] [JsonIgnore] public string FirstName => Name.Split(' ')[0];

        public DateTime LastActivity { get; set; }
        public MemberType Type { get; set; }
        public MemberState State { get; set; }
        public bool IsAdmin => Roles.Any(x => x.Equals("admin"));
        public bool IsCreator => Roles.Any(x => x.Equals("creator"));
        public bool IsGroupAdmin => Roles.Any(x => x.Equals("groupAdmin"));
        public int AverageDifficultyLevel { get; set; }
    }
}
