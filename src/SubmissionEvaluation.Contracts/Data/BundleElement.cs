namespace SubmissionEvaluation.Contracts.Data
{
    public class BundleElement : IElement
    {
        private readonly IBundle bundle;

        public BundleElement(IBundle bundle)
        {
            this.bundle = bundle;
        }

        public string Id => bundle.Id;
        public string Title => bundle.Title;
        public string Category => bundle.Category;
        public bool IsAvailable => !bundle.IsDraft;
        public bool IsBundle => true;
        public RatingMethod RatingMethod => RatingMethod.Fixed;
        public int Activity => bundle.State.Activity;
        public int? DifficultyRating => bundle.State.DifficultyRating;
        public string[] Languages => new string[0]; // TODO: Languages missing!!
        public string LearningFocus => bundle.LearningFocus;
    }
}
