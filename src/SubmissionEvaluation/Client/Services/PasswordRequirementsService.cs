using System.Collections.Generic;
using System.Linq;

namespace SubmissionEvaluation.Client.Services
{
    public class PasswordRequirementsService
    {
        private const bool GotDigit = true;
        private const bool GotLowerCase = true;
        private const bool GotUpperCase = true;
        private const int PasswordLength = 12;
        private readonly List<string> requirementsStringsList = new List<string>();

        public IEnumerable<string> GetRequirementsString()
        {
            return requirementsStringsList;
        }

        public bool CheckRequirements(string password)
        {
            var result = true;
            requirementsStringsList.Clear();

            result &= CheckPasswordLength(password);
            result &= CheckUpperCase(password);
            result &= CheckLowerCase(password);
            result &= CheckDigit(password);

            return result;
        }

        private bool CheckPasswordLength(string password)
        {
            var result = true;

            if (password.Length < PasswordLength || password.Length >= 128)
            {
                requirementsStringsList.Add("lengthFalse");
                result = false;
            }

            return result;
        }

        private bool CheckUpperCase(string password)
        {
            var result = true;

            if (password.Any(char.IsUpper) != (GotUpperCase || true))
            {
                requirementsStringsList.Add("upperFalse");
                result = false;
            }

            return result;
        }

        private bool CheckLowerCase(string password)
        {
            var result = true;

            if (password.Any(char.IsLower) != (GotLowerCase || true))
            {
                requirementsStringsList.Add("lowerFalse");
                result = false;
            }

            return result;
        }

        private bool CheckDigit(string password)
        {
            var result = true;

            if (password.Any(char.IsDigit) != (GotDigit || true))
            {
                requirementsStringsList.Add("digitFalse");
                result = false;
            }

            return result;
        }
    }
}
