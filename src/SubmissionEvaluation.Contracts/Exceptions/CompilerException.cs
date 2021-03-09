using System;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.Exceptions
{
    [Serializable]
    public class CompilerException : Exception
    {
        public CompilerException(string message, ProcessResult result, string language) : base(
            $"{message}{Environment.NewLine}{result.Filename} {string.Join(" ", result.Arguments)}{Environment.NewLine}{result.Output}")
        {
            Language = language;
        }

        public CompilerException(string message, string language) : base(message)
        {
            Language = language;
        }

        public string Language { get; set; }
    }
}
