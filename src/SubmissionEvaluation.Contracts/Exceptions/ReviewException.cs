using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class ReviewException : Exception
    {
        public ReviewException(string message) : base(message)
        {
        }
    }
}
