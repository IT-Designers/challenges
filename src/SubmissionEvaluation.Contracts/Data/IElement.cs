namespace SubmissionEvaluation.Contracts.Data
{
    public interface IElement
    {
        string Id { get; }
        string Title { get; }
        string Category { get; }
        bool IsAvailable { get; }
        bool IsBundle { get; }
        RatingMethod RatingMethod { get; }
        int Activity { get; }
        int? DifficultyRating { get; }
        string[] Languages { get; }
        string LearningFocus { get; }
    }
}
