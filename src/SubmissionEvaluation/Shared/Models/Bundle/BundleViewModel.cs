using System.Collections.Generic;
using SubmissionEvaluation.Contracts.ClientPocos;

namespace SubmissionEvaluation.Shared.Models.Bundle
{
    public class BundleViewModel : GenericModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public bool IsDraft { get; set; }
        public string Description { get; set; }
        public List<Contracts.ClientPocos.Challenge> Challenges { get; set; }
        public Member Member { get; set; }
    }
}
