using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class ChallengeInfoModel : GenericModel
    {
        private readonly IDictionary<string, RatingMethod> converter = new Dictionary<string, RatingMethod>
        {
            {"Fixed", RatingMethod.Fixed},
            {"Score", RatingMethod.Score},
            {"ExecTime", RatingMethod.ExecTime},
            {"SubmissionTime", RatingMethod.SubmissionTime}
        };

        private string ratingMethodInput;

        [Required(ErrorMessage = "Du musst eine ID angeben!")]
        [RegularExpression("^(?!tn_)[a-zA-Z0-9]*$", ErrorMessage = "Die ID darf nicht mit tn_ beginnen und keine Leer- oder Sonderzeichen enthalten!")]
        public string Id { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public string LastEditor { get; set; }
        public string LastEditorId { get; set; }
        public string Category { get; set; }

        public RatingMethod RatingMethod { get; set; }

        //!! Input has to be set at all times after creation. Not possible to use linq here, sadly. 
        public string RatingMethodInput
        {
            get => ratingMethodInput;
            set
            {
                ratingMethodInput = value;
                RatingMethod = converter[value];
            }
        }

        public string Bundle { get; set; }
    }
}
