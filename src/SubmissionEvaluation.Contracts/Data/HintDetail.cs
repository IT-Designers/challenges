using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class HintDetail
    {
        public string Header { get; set; }
        public bool IsHtml { get; set; }
        public string Filename { get; set; }
        public string Content { get; set; }
    }

    public class HintCategory
    {
        public string Title { get; set; }
        public bool IsTabbed { get; set; }
        public List<HintDetail> Hints { get; set; } = new List<HintDetail>();
    }
}
