namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class ApplicationSettings
    {
        public int DelayTime { get; internal set; }
        public string PathToServerWwwRoot { get; set; }
        public string PathToData { get; set; }
        public bool Inactive { get; internal set; }
        public bool DeleteAfterMoreThenOneYearInactivity { get; internal set; }
        public int InstancePort { get; internal set; }
        public string InstanceName { get; internal set; }
        public string WebApiPassphrase { get; internal set; }
        public string SiteUrl { get; internal set; }
    }
}
