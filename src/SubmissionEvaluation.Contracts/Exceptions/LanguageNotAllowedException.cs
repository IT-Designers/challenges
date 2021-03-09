using System;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class LanguageNotAllowedException : Exception
    {
        public LanguageNotAllowedException(string message, string language) : base(message)
        {
            Language = language;
        }

        public string Language { get; private set; }
    }
}
