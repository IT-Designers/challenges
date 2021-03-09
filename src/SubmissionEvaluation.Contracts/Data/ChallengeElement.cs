namespace SubmissionEvaluation.Contracts.Data
{
    public class ChallengeElement : IElement
    {
        private readonly IChallenge challenge;

        public ChallengeElement(IChallenge challenge)
        {
            this.challenge = challenge;
        }

        public string Id => challenge.Id;
        public string Title => challenge.Title;
        public string Category => challenge.Category;
        public bool IsAvailable => challenge.IsAvailable;
        public bool IsBundle => false;
        public RatingMethod RatingMethod => challenge.RatingMethod;
        public int Activity => challenge.State.Activity;
        public int? DifficultyRating => challenge.State.DifficultyRating;
        public string[] Languages => challenge.Languages.ToArray();
        public string LearningFocus => challenge.LearningFocus;
    }
}
