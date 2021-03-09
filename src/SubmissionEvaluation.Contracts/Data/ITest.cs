using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface ITest
    {
        int Id { get; set; }
        string[] Parameters { get; }
        string Hint { get; }
        int Timeout { get; }
        InputDefinition Input { get; }
        List<FileDefinition> InputFiles { get; }
        OutputDefinition ExpectedOutput { get; }
        OutputFileDefinition ExpectedOutputFile { get; }
        CustomTestRunnerDefinition CustomTestRunner { get; }
    }
}
