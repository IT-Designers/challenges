using System.ComponentModel.DataAnnotations;

namespace SubmissionEvaluation.Shared.Models.Account
{
    public class LoginModel : GenericModel
    {
        [Required(ErrorMessage = "Du musst einen Loginnamen angeben!")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Du musst ein Passwort angeben!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
