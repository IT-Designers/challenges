using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface IBundle
    {
        string Id { get; }
        string Title { get; }
        string Category { get; }
        bool IsDraft { get; }
        BundleState State { get; }
        List<string> Challenges { get; }
        string Author { get; }
        string Description { get; }
        bool HasPreviousChallengesCheck { get; }
        string LearningFocus { get; }
    }
}
