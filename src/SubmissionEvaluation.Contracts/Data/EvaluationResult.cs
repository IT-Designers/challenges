namespace SubmissionEvaluation.Contracts.Data
{
    public enum EvaluationResult
    {
        Undefined,
        UnknownError,
        CompilationError,
        NotAllowedLanguage,
        Timeout,
        TestsFailed,
        SucceededWithTimeout,
        Succeeded
    }
}
