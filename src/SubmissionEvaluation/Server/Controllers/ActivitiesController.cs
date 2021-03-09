using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Models.Activities;

namespace SubmissionEvaluation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsChallengePlattformUser")]
    public class ActivitiesController : Controller
    {
        [HttpGet("Activities")]
        public IActionResult Index()
        {
            var activities = JekyllHandler.Domain.Query.GetRecentActivities();
            activities.ForEach(x => x.UserName = JekyllHandler.MemberProvider.GetMemberById(x.UserId)?.Name);
            return Ok(new ActivitiesModel {Entries = activities});
        }
    }
}
