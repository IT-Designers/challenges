using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class DeserializationException : Exception
    {
        public DeserializationException(string message, Exception ex) : base(message, ex)
        {
        }

        public DeserializationException(string message) : base(message)
        {
        }
    }
}
