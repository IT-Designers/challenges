using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Domain.Comparers
{
    public class ChallengeForRankingComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y)
            {
                return 0;
            }

            if (x == "Achievements")
            {
                return 0;
            }

            if (x == "ChallengeCreators")
            {
                return 0;
            }

            if (x == "Reviews")
            {
                return 0;
            }

            if (y == "ChallengeCreators")
            {
                return 0;
            }

            if (y == "Achievements")
            {
                return 0;
            }

            if (y == "Reviews")
            {
                return 0;
            }

            return string.Compare(x, y, StringComparison.Ordinal);
        }
    }
}
