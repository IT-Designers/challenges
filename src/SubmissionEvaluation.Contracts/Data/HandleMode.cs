namespace SubmissionEvaluation.Contracts.Data
{
    public enum HandleMode
    {
        ThrowException,
        ThrowExceptionAndDelete,
        CreateDefaultObject,
        CreateDefaultObjectAndDelete,
        ReturnNull
    }
}
