using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Classes.Config
{
    public class RatingMethodConfig
    {
        public RatingMethod Type { get; internal set; }
        public string Title { get; set; }
        public string Color { get; set; }
    }
}
