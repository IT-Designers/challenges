using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class RunResult
    {
        public string ResultOutput { get; set; }
        public int ExecutionDuration { get; set; }
        public long PeakPagedMem { get; set; }
        public long PeakWorkingSet { get; set; }
        public long PeakVirtualMem { get; set; }
        public ComparisonResult Diff { get; set; }

        public bool IsPassed { get; set; }
        public TestParameters TestParameters { get; set; }
        public string ResultOutputFile { get; set; }
        public ComparisonResult FileDiff { get; set; }
        public bool Timeout { get; set; }
        public int? CustomScore { get; set; }
        public string Commandline { get; set; }
        public string WorkingDirectory { get; set; }
        public List<TestParameters> PreviousTestParemeters { get; set; }
    }
}
