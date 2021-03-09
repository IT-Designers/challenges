using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Bundle
{
    public class BundleModel : GenericModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public bool IsDraft { get; set; }
        public string Description { get; set; }
        public List<string> Challenges { get; set; } = new List<string>();

        public List<Contracts.ClientPocos.Challenge> AvailableChallenges { get; set; } = new List<Contracts.ClientPocos.Challenge>();

        public string SelectedChallenge { get; set; }
        public string SelectedAddChallenge { get; set; }

        public bool HasPreviousChallengesCheck { get; set; }
    }
}
