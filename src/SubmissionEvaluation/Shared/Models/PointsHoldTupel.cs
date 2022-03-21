using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models
{
    public class PointsHoldTupel<T, TS> where T : ISubmission where TS : IMember
    {
        public PointsHoldTupel(SubmitterRankingEntry entry, T submission, T duplicatedFrom, TS authorOfDuplication)
        {
            Entry = entry;
            Submission = submission;
            DuplicatedFrom = duplicatedFrom;
            AuthorOfDuplication = authorOfDuplication;
        }

        public SubmitterRankingEntry Entry { get; set; }
        public T Submission { get; set; }
        public T DuplicatedFrom { get; set; }
        public TS AuthorOfDuplication { get; set; }
    }
}
