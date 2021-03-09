using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Comparers
{
    public class IChallengeComparer : EqualityComparer<IChallenge>
    {
        public override bool Equals(IChallenge x, IChallenge y)
        {
            return x.Id.Equals(y.Id);
        }

        public override int GetHashCode(IChallenge obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
