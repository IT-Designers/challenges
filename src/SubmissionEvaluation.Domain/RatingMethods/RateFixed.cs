using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.RatingMethods
{
    internal class RateFixed : ISubmissionRater
    {
        public RatingMethod Name => RatingMethod.Fixed;

        public int Compare(SubmissionEntry x, SubmissionEntry y)
        {
            return x.Points - y.Points;
        }

        public void UpdatePoints(List<SubmissionEntry> submitters, RatingPoints points)
        {
            foreach (var submitter in submitters)
            {
                submitter.Points = points.Mid;
            }
        }

        public void UpdateRanking(List<SubmissionEntry> submitters)
        {
            foreach (var submitter in submitters)
            {
                submitter.Rank = 1;
            }
        }
    }
}
