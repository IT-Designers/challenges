using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class Output
    {
        public string Content { get; set; }
        public List<string> Alternatives { get; set; }
        public CompareSettingsModel CompareSettings { get; set; } = new CompareSettingsModel();
    }
}
