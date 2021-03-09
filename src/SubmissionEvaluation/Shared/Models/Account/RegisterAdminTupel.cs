namespace SubmissionEvaluation.Shared.Models.Account
{
    public class RegisterAdminTupel
    {
        public RegisterAdminTupel(RegisterModel model, string token)
        {
            Model = model;
            Token = token;
        }

        public RegisterModel Model { get; set; }
        public string Token { get; set; }
    }
}
