using System;
using System.Linq;

namespace SubmissionEvaluation.Domain
{
    internal static class DuplicateChecker
    {
        /// <summary>Determines the similarity between two strings from 0.0 to 1.0</summary>
        public static double DetermineProbabilityOfDuplicate(this string s, string t)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(t))
            {
                return 0.0;
            }

            if (s.Equals(t))
            {
                return 1.0;
            }

            return 1.0 - LevenshteinDistance(s, t) / (double) Math.Max(s.Length, t.Length);
        }

        /// <summary>
        ///     Implementation of Levenshtein distance as given in
        ///     "Iterative with two matrix rows" (https://en.wikipedia.org/wiki/Levenshtein_distance)
        /// </summary>
        private static int LevenshteinDistance(string s, string t)
        {
            // create two work vectors of integer distances
            var v0 = new int[t.Length + 1];
            var v1 = new int[t.Length + 1];

            // initialize v0 (the previous row of distances)
            // this row is A[0][i]: edit distance for an empty s
            // the distance is just the number of characters to delete from t
            for (var i = 0; i <= t.Length; i++)
            {
                v0[i] = i;
            }

            for (var i = 0; i < s.Length; i++)
            {
                // calculate v1 (current row distances) from the previous row v0

                // first element of v1 is A[i+1][0]
                //   edit distance is delete (i+1) chars from s to match empty t
                v1[0] = i + 1;

                // use formula to fill in the rest of the row
                for (var j = 0; j < t.Length; j++)
                {
                    // calculating costs for A[i+1][j+1]
                    var deletionCost = v0[j + 1] + 1;
                    var insertionCost = v1[j] + 1;
                    var substitutionCost = s[i].Equals(t[j]) ? v0[j] : v0[j] + 1;
                    v1[j + 1] = new[] {deletionCost, insertionCost, substitutionCost}.Min();
                }

                // copy v1 (current row) to v0 (previous row) for next iteration
                // since data in v1 is always invalidated, a swap without copy could be more efficient
                var tempRef = v0;
                v0 = v1;
                v1 = tempRef;
            }

            // after the last swap, the results of v1 are now in v0
            return v0[t.Length];
        }
    }
}
