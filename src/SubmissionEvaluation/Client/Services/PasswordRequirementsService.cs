using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubmissionEvaluation.Client.Services
{
    public class PasswordRequirementsService
    {
        readonly int passwordLength = 12;
        readonly bool gotUpperCase = true;
        readonly bool gotLowerCase = true;
        readonly bool gotDigit = true;
        List<string> requirementsStringsList = new List<string>();

        public List<string> GetRequirementsString()
        {
            return requirementsStringsList;
        }

        public bool CheckRequirements(string password)
        {
            bool result = true;
            requirementsStringsList.Clear();

            result &= CheckPasswordLength(password);
            result &= CheckUpperCase(password);
            result &= CheckLowerCase(password);
            result &= CheckDigit(password);

            return result;
        }

        private bool CheckPasswordLength(string password)
        {
            bool result = true;

            if(password.Length < passwordLength || password.Length >= 128)
            {
                requirementsStringsList.Add("lengthFalse");
                result = false;
            }

            return result;
        }
        private bool CheckUpperCase(string password)
        {
            bool result = true;

            if (password.Any(char.IsUpper) != (gotUpperCase || true))
            {
                requirementsStringsList.Add("upperFalse");
                result = false;
            }

            return result;
        }
        private bool CheckLowerCase(string password)
        {
            bool result = true;

            if (password.Any(char.IsLower) != (gotLowerCase || true))
            {
                requirementsStringsList.Add("lowerFalse");
                result = false;
            }

            return result;
        }
        private bool CheckDigit(string password)
        {
            bool result = true;

            if (password.Any(char.IsDigit) != (gotDigit || true))
            {
                requirementsStringsList.Add("digitFalse");
                result = false;
            }

            return result;
        }
    }
}
