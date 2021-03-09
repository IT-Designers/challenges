using SubmissionEvaluation.Classes.Config;

namespace SubmissionEvaluation.Shared.Models
{
    public class CategoryListEntryModel
    {
        public string Title { get; set; }
        public bool IsBundle { get; set; }
        public string Id { get; set; }
        public string Languages { get; set; }
        public RatingMethodConfig RatingMethod { get; set; }
        public bool IsPartOfBundle { get; set; }
        public int? DifficultyRating { get; set; }
        public string LearningFocus { get; set; }
    }
}
