using System.ComponentModel.DataAnnotations;

namespace SubmissionEvaluation.Shared.Models.Account
{
    public class ChangePasswordModel : GenericModel
    {
        [Required(ErrorMessage = "Du musst das alte Passwort angeben!")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Du musst das neue Passwort angeben!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Das neue Passwort ist nicht gleich!")]
        public string ConfirmPassword { get; set; }
    }
}
