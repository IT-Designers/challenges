using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Bundle
{
    public class BundleOverviewModel : ProfileHeaderModel
    {
        public List<BundleModel> Bundles { get; set; } = new List<BundleModel>();
        public IDictionary<string, string> Categories { get; set; } = new Dictionary<string, string>();
    }
}
