using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using Member = SubmissionEvaluation.Contracts.ClientPocos.Member;

namespace SubmissionEvaluation.Shared.Models
{
    public class IndexHomeModel
    {
        public List<Activity> Activities { get; set; } = new List<Activity>();

        public Dictionary<string, List<CategoryListEntryExtendedModel>> CategoryStats { get; set; } =
            new Dictionary<string, List<CategoryListEntryExtendedModel>>();

        //public List<BundleViewModel> Bundles { get; set; } = new List<BundleViewModel>();
        public Member Member { get; set; } = null;
    }
}
