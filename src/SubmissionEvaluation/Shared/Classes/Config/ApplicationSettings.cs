namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class ApplicationSettings
    {
        public int Delaytime { get; internal set; }
        public string PathToLogger { get; internal set; }
        public string PathToServerWwwRoot { get; set; }
        public string PathToData { get; set; }
        public bool Inactive { get; internal set; }
        public int InstancePort { get; internal set; }
        public string InstanceName { get; internal set; }
        public string WebApiPassphrase { get; internal set; }
        public string SiteUrl { get; internal set; }
    }
}
