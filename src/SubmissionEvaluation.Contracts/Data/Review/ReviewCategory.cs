using System.Diagnostics.CodeAnalysis;

namespace SubmissionEvaluation.Contracts.Data.Review
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class ReviewCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantifier { get; set; } = 1;
    }
}
