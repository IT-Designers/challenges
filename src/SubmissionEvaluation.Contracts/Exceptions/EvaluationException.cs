using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class EvaluationException : Exception
    {
        public EvaluationException(string message) : base(message)
        {
        }
    }
}
