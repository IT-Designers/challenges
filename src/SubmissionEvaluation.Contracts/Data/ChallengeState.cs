using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class ChallengeState
    {
        public int PassedCount { get; set; }
        public int FailedCount { get; set; }

        [YamlIgnore] [JsonIgnore] public int SubmissionsCount => PassedCount + FailedCount;

        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }
        public string LastEditorId { get; set; }
        public int FeasibilityIndex { get; set; }
        public int FeasibilityIndexMod { get; set; }
        public int? DifficultyRating { get; set; }
        public bool IsPartOfBundle { get; set; }
        public string MinEffort { get; set; }
        public string MaxEffort { get; set; }
        public List<string> Features { get; set; }
        public int Activity { get; set; }
    }
}
