using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models
{
    public class PointsHoldTupel<T, S> where T : ISubmission where S : IMember
    {
        public PointsHoldTupel(SubmitterRankingEntry entry, T submission, T duplicatedFrom, S authorOfDuplication)
        {
            Entry = entry;
            Submission = submission;
            DuplicatedFrom = duplicatedFrom;
            AuthorOfDuplication = authorOfDuplication;
        }

        public SubmitterRankingEntry Entry { get; set; }
        public T Submission { get; set; }
        public T DuplicatedFrom { get; set; }
        public S AuthorOfDuplication { get; set; }
    }
}
