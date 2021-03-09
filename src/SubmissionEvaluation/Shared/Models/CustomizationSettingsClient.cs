using System.Collections.Generic;
using SubmissionEvaluation.Classes.Config;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Shared.Models
{
    public class CustomizationSettingsClient
    {
        public Dictionary<string, AchievementConfig> Achievements { get; set; }
        public Dictionary<string, ResultConfig> Results { get; set; }
        public Dictionary<string, RatingMethodConfig> RatingMethods { get; set; }
        public Dictionary<string, string> Categories { get; set; }
    }
}
