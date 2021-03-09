using System.ComponentModel.DataAnnotations;

namespace SubmissionEvaluation.Shared.Models.Account
{
    public class RegisterModel : GenericModel
    {
        [EmailAddress] public string Mail { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)] public string Password { get; set; }

        public string Fullname { get; set; }
    }
}
