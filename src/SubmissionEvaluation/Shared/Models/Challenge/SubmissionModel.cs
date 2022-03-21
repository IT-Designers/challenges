using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class SubmissionModel<T, TS> where T : ISubmission where TS : IMember
    {
        public T Submission { get; set; }
        public TS Member { get; set; }
    }
}
