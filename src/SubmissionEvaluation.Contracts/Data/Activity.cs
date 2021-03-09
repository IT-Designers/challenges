using System;

namespace SubmissionEvaluation.Contracts.Data
{
    public class Activity
    {
        public DateTime Date { get; set; }
        public ActivityType Type { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Reference { get; set; }
    }
}
