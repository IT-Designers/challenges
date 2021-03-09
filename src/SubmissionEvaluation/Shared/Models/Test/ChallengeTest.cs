using System.Collections.Generic;
using SubmissionEvaluation.Shared.Models.Shared;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class ChallengeTest
    {
        public int Id { get; set; }
        public string Index { get; set; } = "";
        public string Hint { get; set; } = "";
        public List<string> Parameters { get; set; }
        public string Input { get; set; } = "";
        public Output Output { get; set; } = new Output();
        public OutputFile OutputFile { get; set; } = new OutputFile();
        public int Timeout { get; set; } = 5;
        public List<File> InputFiles { get; set; } = new List<File>();
        public string CustomTestRunnerName { get; set; } = "";
    }
}
