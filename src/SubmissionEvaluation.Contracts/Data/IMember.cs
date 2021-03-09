using System;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface IMember
    {
        string Name { get; }
        string Mail { get; }
        string Id { get; }
        string Uid { get; }
        DateTime DateOfEntry { get; }
        bool IsReviewer { get; }
        int ReviewCounter { get; }
        ReviewLevel ReviewLevel { get; }
        int ReviewFrequency { get; }
        DateTime LastReview { get; }
        string[] ReviewLanguages { get; }
        string FirstName { get; }
        DateTime LastActivity { get; }
        string[] Roles { get; }
        string[] Groups { get; }
        string[] UnlockedChallenges { get; }
        string[] SolvedChallenges { get; }
        string[] CanRate { get; }
        string Password { get; }
        DateTime? LastNotificationCheck { get; }
        MemberType Type { get; }
        MemberState State { get; }
        bool IsAdmin { get; }
        bool IsCreator { get; }
        bool IsGroupAdmin { get; }
        int AverageDifficultyLevel { get; }
    }
}
