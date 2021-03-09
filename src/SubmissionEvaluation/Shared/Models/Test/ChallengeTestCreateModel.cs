using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class ChallengeTestCreateModel : GenericModel
    {
        public string ChallengeId { get; set; }
        public ChallengeTest Test { get; set; }
        public List<string> InputFileEntries { get; set; }
        public bool IsUserAdmin { get; set; } = false;
    }
}
