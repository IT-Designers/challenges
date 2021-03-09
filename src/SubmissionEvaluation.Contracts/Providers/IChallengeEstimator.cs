using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface IChallengeEstimator
    {
        Effort GuessEffortFor(Challenge challenge, IEnumerable<Result> results, double? lastEstimationRating);
        List<string> FindFeaturesFor(IEnumerable<Result> results);
    }
}
