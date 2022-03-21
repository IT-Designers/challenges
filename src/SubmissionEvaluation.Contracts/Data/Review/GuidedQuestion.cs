using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class GuidedQuestion
    {
        public const int NoProblems = 0;
        public const int MinimalProblems = 1;
        public const int SubmissionShouldBeImproved = 2;
        public const int NoEvaluation = 3;

        public GuidedQuestion()
        {
            Issues ??= new Issue[0];
        }

        public string Id { get; set; }
        public string Question { get; set; }

        public Issue[] Issues { get; set; }

        //Used for Rating value in Review.
        public int? Rating { get; set; }

        public static Dictionary<int, string> RatingToDescription { get; } = new Dictionary<int, string>
        {
            {3, "Nicht Bewerten"},
            {0, "Keine Probleme vorhanden"},
            {1, "Minimale Probleme(Nachbesserung nicht notwendig)"},
            {2, "Probleme Vorhanden(Sollte nachgebessert werden)"}
        };
    }
}
