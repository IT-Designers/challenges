using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.Operations
{
    internal class ProviderStore
    {
        public IFileProvider FileProvider { get; set; }
        public IProcessProvider ProcessProvider { get; set; }
        public IProcessProvider SandboxedProcessProvider { get; set; }
        public ILog Log { get; set; }
        public IMemberProvider MemberProvider { get; set; }
        public string SiteUrl { get; set; }
        public string HelpEmail { get; set; }
    }
}
