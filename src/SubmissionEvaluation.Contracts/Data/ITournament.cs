using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface ITournament
    {
        string Name { get; }
        string AuthorId { get; }
        string LastEditorId { get; }
        string Title { get; }
        bool HasError { get; }
        bool IsDraft { get; }
        DateTime Date { get; }
        DateTime Startdate { get; }
        DateTime Enddate { get; }
        List<string> Languages { get; }
        string Description { get; }
        string Game { get; }
        string ErrorDescription { get; }
        List<string> LimitedFor { get; }
    }
}
