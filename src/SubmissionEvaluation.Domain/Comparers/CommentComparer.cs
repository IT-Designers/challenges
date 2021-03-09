using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Domain.Comparers
{
    internal class CommentComparer : IEqualityComparer<ReviewComments>
    {
        public bool Equals(ReviewComments x, ReviewComments y)
        {
            return x.Text == y.Text;
        }

        public int GetHashCode(ReviewComments obj)
        {
            return obj.Text?.GetHashCode() ?? 0;
        }
    }
}
