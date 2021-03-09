namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal interface IDiffCreator
    {
        (bool Success, string Details, bool showExpected) GetDiff(string submission, string solution);
    }
}
