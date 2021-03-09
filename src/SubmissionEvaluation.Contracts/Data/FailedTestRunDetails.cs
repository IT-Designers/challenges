using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class FailedTestRunDetails
    {
        public string HintMessage { get; set; }
        public List<HintCategory> HintCategories { get; set; }
        public string ErrorMessage { get; set; }
    }
}
