namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class LdapSettings
    {
        public string Ip { get; internal set; }
        public int Port { get; internal set; }
        public string Domain { get; internal set; }
        public string OrganizationalUnit { get; internal set; }
        public string AccessUser { get; internal set; }
        public string AccessPassword { get; internal set; }
        public string UidAttribute { get; internal set; }
    }
}
