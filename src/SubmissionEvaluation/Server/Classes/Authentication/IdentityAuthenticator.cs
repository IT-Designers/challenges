using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Interfaces;
using SubmissionEvaluation.Server.Classes.JekyllHandling;

namespace SubmissionEvaluation.Server.Classes.Authentication
{
    public class IdentityAuthenticator : IAuthenticator
    {
        public async Task WriteIdentityCookie(string username, Dictionary<string, string> attributeTable, HttpContext httpContext)
        {
            var fullname = $"{attributeTable["givenName"]} {attributeTable["sn"]}";
            var mail = attributeTable["mail"].ToLower();
            var foundMember = JekyllHandler.GetMemberByUid(username);
            if (foundMember == null)
            {
                foundMember = JekyllHandler.MemberProvider.GetMemberByMail(mail);
                if (foundMember == null)
                {
                    foundMember = JekyllHandler.MemberProvider.GetMemberByName(fullname);
                }

                if (foundMember != null)
                {
                    JekyllHandler.MemberProvider.UpdateUid(foundMember, attributeTable["uid"]);
                    JekyllHandler.MemberProvider.LogLastActivity(foundMember);
                }
            }

            if (foundMember == null)
            {
                foundMember = JekyllHandler.Domain.Interactions.AddMember(fullname, mail, attributeTable["uid"]);
            }

            await LoginForMember(foundMember, foundMember.Type.ToString(), httpContext);
        }

        public static async Task LoginForMember(IMember foundMember, string method, HttpContext httpContext)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.GivenName, "cn=" + foundMember.FirstName),
                new Claim(ClaimTypes.Name, foundMember.Name),
                new Claim(ClaimTypes.AuthenticationMethod, method),
                new Claim(ClaimTypes.NameIdentifier, foundMember.Uid ?? ""),
                new Claim(ClaimTypes.Email, foundMember.Mail),
                new Claim(ClaimTypes.Sid, foundMember.Id)
            };

            foreach (var role in foundMember.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            if (foundMember.IsReviewer)
            {
                claims.Add(new Claim(ClaimTypes.Role, "reviewer"));
            }

            var claimsIdentity = new ClaimsIdentity(claims, "Basic");
            var claimsPrinciple = new ClaimsPrincipal(claimsIdentity);

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrinciple);
        }
    }
}
