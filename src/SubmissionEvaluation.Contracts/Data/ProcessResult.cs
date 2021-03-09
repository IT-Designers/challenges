namespace SubmissionEvaluation.Contracts.Data
{
    public class ProcessResult
    {
        public ModifiedFile[] ModifiedFiles { get; set; } = new ModifiedFile[0];
        public int? ExitCode { get; set; }
        public string Output { get; set; }
        public bool Timeout { get; set; }
        public string Filename { get; set; }
        public string[] Arguments { get; set; }
        public string WorkingDir { get; set; }
        public int ExecutionDuration { get; set; }
        public bool Exception { get; set; }

        public override string ToString()
        {
            return $"{Filename} {Arguments} (Exitcode: {ExitCode})\nDetails: {Output}";
        }
    }
}
