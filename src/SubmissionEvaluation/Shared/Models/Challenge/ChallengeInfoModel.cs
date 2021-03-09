using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class ChallengeInfoModel : GenericModel
    {
        private readonly IDictionary<string, RatingMethod> Converter = new Dictionary<string, RatingMethod>
        {
            {"Fixed", RatingMethod.Fixed},
            {"Score", RatingMethod.Score},
            {"Exec_Time", RatingMethod.Exec_Time},
            {"Submission_Time", RatingMethod.Submission_Time}
        };

        private string _ratingMethodInput;

        [Required(ErrorMessage = "Du musst eine ID angeben!")]
        [RegularExpression("^(?!tn_)[a-zA-Z0-9]*$", ErrorMessage = "Die ID darf nicht mit tn_ beginnen und keine Leer- oder Sonderzeichen enthalten!")]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string AuthorID { get; set; }
        public string LastEditor { get; set; }
        public string LastEditorID { get; set; }
        public string Category { get; set; }

        public RatingMethod RatingMethod { get; set; }

        //!! Input has to be set at all times after creation. Not possible to use linq here, sadly. 
        public string RatingMethodInput
        {
            get => _ratingMethodInput;
            set
            {
                _ratingMethodInput = value;
                RatingMethod = Converter[value];
            }
        }

        public string Bundle { get; set; }
    }
}
