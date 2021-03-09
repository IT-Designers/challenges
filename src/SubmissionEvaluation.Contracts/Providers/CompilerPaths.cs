namespace SubmissionEvaluation.Contracts.Providers
{
    public class CompilerPaths
    {
        public BuildPaths Host { get; } = new BuildPaths();
        public BuildPaths Sandbox { get; } = new BuildPaths();
    }
}
