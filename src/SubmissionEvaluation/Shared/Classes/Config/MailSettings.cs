namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class MailSettings
    {
        public string Username { get; internal set; }
        public string Password { get; internal set; }
        public string SmtpServer { get; internal set; }
        public string SendMailAddress { get; internal set; }
        public string HelpMailAddress { get; internal set; }
        public int Port { get; internal set; }
    }
}
