using System.Collections.Generic;
using SubmissionEvaluation.Classes.Config;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class CustomizationSettings
    {
        public IDictionary<string, AchievementConfig> Achievements { get; internal set; }
        public Dictionary<string, ResultConfig> Results { get; internal set; }
        public IDictionary<RatingMethod, RatingMethodConfig> RatingMethods { get; internal set; }
        public Dictionary<string, string> Categories { get; internal set; }
    }
}
