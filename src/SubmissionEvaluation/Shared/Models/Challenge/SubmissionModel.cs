using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class SubmissionModel<T, S> where T : ISubmission where S : IMember
    {
        public T Submission { get; set; }
        public S Member { get; set; }
    }
}
