using SubmissionEvaluation.Shared.Classes.Messages;

namespace SubmissionEvaluation.Shared.Models
{
    public class GenericModel
    {
        public bool HasError { get; set; }
        public bool HasSuccess { get; set; }
        public string Message { get; set; }

        public string MessageTranslation => Translation.GetMessage(this);

        public string AlertColor
        {
            get
            {
                if (HasError)
                {
                    return "alert-danger";
                }

                if (HasSuccess)
                {
                    return "alert-success";
                }

                return "alert-warning";
            }
        }

        public string Referer { get; set; }

        public string Content { get; set; }
    }
}
