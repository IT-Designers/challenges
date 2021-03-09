using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class RunnerPropeties
    {
        public string Command { get; set; }
        public string Path { get; set; }
        public List<string> DataPaths { get; set; }
        public List<string> Parameters { get; set; }
    }
}
