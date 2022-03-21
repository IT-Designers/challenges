using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class ReviewModel : GenericModel
    {
        public class Compiler {
            public string Name { get; set; }
            public bool Selected { get; set; }
        }

        public List<Compiler> Compilers { get; set; }
    }
}
