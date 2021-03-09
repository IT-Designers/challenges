using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Help
{
    public class HelpModel : GenericModel
    {
        public string Description { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public List<HelpPage> Pages { get; set; }
        public string Parent { get; set; }
    }
}
