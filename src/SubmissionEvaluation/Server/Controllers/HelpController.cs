using System;
using Microsoft.AspNetCore.Mvc;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;
using SubmissionEvaluation.Shared.Models.Help;

namespace SubmissionEvaluation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelpController : Controller
    {
        [HttpPost("getViewPage")]
        public IActionResult ViewPage([FromBody] string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = "index";
                }

                if (path.Contains(".."))
                {
                    throw new Exception("Illegal path");
                }

                var helpPage = JekyllHandler.Domain.Query.GetHelpPage(path);
                var helpHierarchie = JekyllHandler.Domain.Query.GetHelpHierarchy();
                return Ok(new HelpModel
                {
                    Path = path,
                    Description = helpPage.Description.Replace("@Settings.Mail.HelpMailAddress", Settings.Mail.HelpMailAddress),
                    Title = helpPage.Title,
                    Pages = helpHierarchie
                });
            }
            catch (Exception)
            {
                var helpPage = JekyllHandler.Domain.Query.GetHelpPage("index");
                return Ok(new HelpModel {Path = path, Description = helpPage.Description, Title = helpPage.Title});
            }
        }
    }
}
