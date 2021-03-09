namespace SubmissionEvaluation.Contracts.Data
{
    public interface IStarterpack
    {
        string Name { get; }
        string Description { get; }
        string Language { get; }
        string Filename { get; }
    }
}
