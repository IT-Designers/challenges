using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface ISubmissionRater : IComparer<SubmissionEntry>
    {
        RatingMethod Name { get; }
        void UpdatePoints(List<SubmissionEntry> submitters, RatingPoints points);
        void UpdateRanking(List<SubmissionEntry> submitters);
    }
}
