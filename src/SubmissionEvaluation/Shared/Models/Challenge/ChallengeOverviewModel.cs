using System.Collections.Generic;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class ChallengeOverviewModel : GenericModel
    {
        public IReadOnlyList<ChallengeModel> Challenges { get; set; }
        public Dictionary<string, RatingMethodConfig> RatingMethods { get; set; }
        public Dictionary<string, string> Categories { get; set; }
    }
}
