using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface IGame
    {
        string AuthorID { get; }
        string LastEditorID { get; }
        string Name { get; }
        string Title { get; }
        bool IsDraft { get; }
        List<List<string>> Starterpacks { get; }
        string StarterpacksIntroduction { get; }
        DateTime Date { get; }
        List<string> Languages { get; }
        string Description { get; }
        bool HasError { get; }
        string ErrorDescription { get; }
        List<string> Files { get; }
        RunnerPropeties Verifier { get; }
        RunnerPropeties MatchRunner { get; }
        List<int> PossiblePlayersPerMatch { get; }
        int MatchTimeout { get; }
    }
}
