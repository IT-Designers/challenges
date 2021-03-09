using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class EvaluationTimeoutException : Exception
    {
        public EvaluationTimeoutException(string message) : base(message)
        {
        }
    }
}
