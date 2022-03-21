using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace SubmissionEvaluation.Server.Contracts.Interfaces
{
    internal interface IUserAuthentication
    {
        void SetLogger(ILogger logger);
        Dictionary<string, string> VerifyUser(string username, string password);
    }
}
