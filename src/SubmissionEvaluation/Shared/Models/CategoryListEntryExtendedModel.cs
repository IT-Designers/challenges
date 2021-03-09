namespace SubmissionEvaluation.Shared.Models
{
    public class CategoryListEntryExtendedModel : CategoryListEntryModel
    {
        public int Activity { get; set; }

        public string Category { get; set; }

        public bool IsAvailable { get; set; }
    }
}
