using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.RatingMethods
{
    internal class RateSubmissionTime : ISubmissionRater
    {
        public RatingMethod Name => RatingMethod.Submission_Time;

        public int Compare(SubmissionEntry x, SubmissionEntry y)
        {
            return y.Date.CompareTo(x.Date);
        }

        public void UpdatePoints(List<SubmissionEntry> submitters, RatingPoints points)
        {
            // Points for best 33% for best 66% (34-66) and for the rest (67+)
            var pointsForBest33 = points.Best;
            var pointsForBest66 = points.Mid;
            var pointsForBest100 = points.Last;

            var submitterCount = submitters.Count;

            // if only 1 submitter give max points
            if (submitterCount == 1)
            {
                submitters[0].Points = pointsForBest33;
                return;
            }

            // if only 2 submitters give 1. max 2. min
            if (submitterCount == 2)
            {
                submitters[0].Points = pointsForBest33;
                if (submitters[0].Date == submitters[1].Date)
                {
                    submitters[1].Points = pointsForBest33;
                }
                else
                {
                    submitters[1].Points = pointsForBest66;
                }

                return;
            }

            double index = 1;

            foreach (var submitter in submitters)
            {
                if (index / submitterCount < 0.34)
                {
                    submitter.Points = pointsForBest33;
                }
                else if (index / submitterCount < 0.67)
                {
                    if (submitters[(int) index - 2].Date == submitter.Date && submitters[(int) index - 2].Points == pointsForBest33)
                    {
                        submitter.Points = pointsForBest33;
                    }
                    else
                    {
                        submitter.Points = pointsForBest66;
                    }
                }
                else
                {
                    if (submitters[(int) index - 2].Date == submitter.Date && submitters[(int) index - 2].Points == pointsForBest33)
                    {
                        submitter.Points = pointsForBest33;
                    }
                    else if (submitters[(int) index - 2].Date == submitter.Date && submitters[(int) index - 2].Points == pointsForBest66)
                    {
                        submitter.Points = pointsForBest66;
                    }
                    else
                    {
                        submitter.Points = pointsForBest100;
                    }
                }

                index++;
            }
        }

        public void UpdateRanking(List<SubmissionEntry> submitters)
        {
            foreach (var submitter in submitters)
            {
                var rank = 1 + submitters.Count(rival => rival.Date < submitter.Date);
                submitter.Rank = rank;
            }
        }
    }
}
