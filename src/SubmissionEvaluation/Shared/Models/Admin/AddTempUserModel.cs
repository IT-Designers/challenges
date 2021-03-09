using System.ComponentModel.DataAnnotations;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class AddTempUserModel
    {
        [Required(ErrorMessage = "Name muss angegeben werden")]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Required] [EmailAddress] public string Mail { get; set; }
    }

    public class ResetPasswordModel<T> where T : IMember
    {
        public T Member { get; set; }

        [DataType(DataType.Password)] public string Password { get; set; }
    }
}
