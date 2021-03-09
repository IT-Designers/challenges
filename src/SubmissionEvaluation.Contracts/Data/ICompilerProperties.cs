using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public interface ICompilerProperties
    {
        List<string> Languages { get; }
    }
}
