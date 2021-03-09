using System;
using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models
{
    public class NotificationModel
    {
        public string Count { get; set; }
        public IEnumerable<Notification> Notifications { get; set; }
    }

    public class Notification
    {
        public string Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public string SourceUrl { get; set; }
        public bool IsNew { get; set; }
        public string Image { get; set; }
    }
}
