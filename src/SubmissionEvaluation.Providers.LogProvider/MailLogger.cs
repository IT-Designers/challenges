using System;
using System.Collections.Generic;
using Serilog.Events;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Providers.LogProvider
{
    internal class MailLogger : IObserver<LogEvent>
    {
        private readonly List<string> lastLogEntries = new List<string>();
        private readonly List<string> lastLogErrorEntries = new List<string>();
        private readonly object syncKey = new object();
        private DateTime lastSendMail;
        private IList<string> reportMails = new List<string>();
        private ISmtpProvider smtpProvider;

        public IList<string> ReportMails
        {
            set
            {
                lock (syncKey)
                {
                    reportMails = value;
                }
            }
        }

        public ISmtpProvider SmtpProvider
        {
            set
            {
                lock (syncKey)
                {
                    smtpProvider = value;
                }
            }
        }

        public void OnNext(LogEvent value)
        {
            lock (syncKey)
            {
                lastLogEntries.Add($"{value.Timestamp.DateTime}[{value.Level}]{value.RenderMessage()} {value.Exception}");

                var reportEntry = (value.Level == LogEventLevel.Error || value.Level == LogEventLevel.Fatal) && (value.Exception == null ||
                    value.Exception.GetType().FullName != "Microsoft.AspNetCore.Antiforgery.AntiforgeryValidationException");
                if (reportEntry)
                {
                    var errorlog = string.Join(Environment.NewLine, lastLogEntries);
                    lastLogErrorEntries.Add(errorlog + Environment.NewLine + "-----8<-----" + Environment.NewLine);
                    lastLogEntries.Clear();
                }

                while (lastLogEntries.Count > 20)
                {
                    lastLogEntries.RemoveAt(0);
                }

                if (reportEntry)
                {
                    if (value.Level == LogEventLevel.Error && lastSendMail.AddHours(6) < DateTime.Now ||
                        value.Level == LogEventLevel.Fatal && lastSendMail.AddHours(2) < DateTime.Now)
                    {
                        SendReportMail();
                    }
                }
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }

        private void SendReportMail()
        {
            if (smtpProvider != null && lastLogErrorEntries.Count > 0)
            {
                try
                {
                    foreach (var mail in reportMails)
                    {
                        smtpProvider.SendMail(mail, "Challenge: Errorreport of " + lastLogErrorEntries.Count + " errors",
                            string.Join(Environment.NewLine, lastLogErrorEntries.ToArray()));
                    }
                }
                finally
                {
                    lastSendMail = DateTime.Now;
                    lastLogErrorEntries.Clear();
                }
            }
        }

        public void SendDelayedErrorReport()
        {
            lock (syncKey)
            {
                if (lastSendMail.AddHours(6) < DateTime.Now)
                {
                    SendReportMail();
                }
            }
        }
    }
}
