using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class ChallengeLockedForUserException : Exception
    {
        public ChallengeLockedForUserException(string message) : base(message)
        {
        }
    }
}
