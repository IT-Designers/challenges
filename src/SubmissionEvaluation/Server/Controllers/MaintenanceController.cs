using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server.Controllers
{
    [Route("api/[controller]")]
    public class MaintenanceController : Controller
    {
        [HttpGet]
        [Route("State/{key}")]
        public JsonResult GetState(string key)
        {
            if (key != Settings.Application.WebApiPassphrase)
            {
                throw new Exception("Invalid passphrase " + key);
            }

            var member = JekyllHandler.GetMemberForUser(User);

            var path = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName;
            return Json(new
            {
                Name = Settings.Application.InstanceName,
                Port = Settings.Application.InstancePort,
                Path = path,
                Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion,
                WebPagesGenerated = System.IO.File.Exists(Path.Combine(path, "WebHost", "wwwroot", "index.html")),
                ChallengesCount = JekyllHandler.Domain.Query.GetAllChallenges(member).Count,
                SubmissionsCount =
                    JekyllHandler.Domain.Query.GetAllChallenges(member).Select(x => JekyllHandler.Domain.Query.GetAllSubmissionsFor(x).Count).Sum(),
                MembersCount = JekyllHandler.MemberProvider.GetMembers().Count(),
                Maintenance = JekyllHandler.Domain.IsMaintenanceMode
            });
        }

        [HttpPut]
        [Route("Mode/{key}")]
        public void PutMaintenanceMode(string key, [FromBody] bool enable)
        {
            if (key != Settings.Application.WebApiPassphrase)
            {
                throw new Exception("Invalid passphrase " + key);
            }

            if (enable)
            {
                Task.Run(() => SchedulesAndTasks.EnableMaintenanceMode());
            }
            else
            {
                Task.Run(() => SchedulesAndTasks.DisableMaintenanceMode());
            }
        }

        [HttpPost]
        [Route("Shutdown/{key}")]
        public void PostShutdown(string key)
        {
            if (key != Settings.Application.WebApiPassphrase)
            {
                throw new Exception("Invalid passphrase " + key);
            }

            Task.Run(() => SchedulesAndTasks.Shutdown());
        }
    }
}
