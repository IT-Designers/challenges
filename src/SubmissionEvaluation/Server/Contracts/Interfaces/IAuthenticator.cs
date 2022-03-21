using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SubmissionEvaluation.Server.Contracts.Interfaces
{
    internal interface IAuthenticator
    {
        Task WriteIdentityCookie(string username, Dictionary<string, string> attributeTable, HttpContext httpContext);
    }
}
