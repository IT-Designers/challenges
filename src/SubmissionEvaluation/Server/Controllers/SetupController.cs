using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Server.Classes;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;
using SubmissionEvaluation.Shared.Classes.Messages;
using SubmissionEvaluation.Shared.Models;
using SubmissionEvaluation.Shared.Models.Account;

namespace SubmissionEvaluation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetupController : Controller
    {
        private readonly ILogger _logger;

        public SetupController(ILogger<SetupController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("TokenValid")]
        public IActionResult CheckToken([FromBody] string token)
        {
            return Ok(CheckTokenValidity(token));
        }

        private static bool CheckTokenValidity(string token)
        {
            return token.Equals(Settings.SecurityToken);
        }

        [AllowAnonymous]
        [HttpPost("RegisterFirstAdmin")]
        public IActionResult RegisterFirstAdmin([FromBody] RegisterAdminTupel input)
        {
            //Input array contains the registration model at index 0 and the security token at index 1.
            var model = new GenericModel {HasSuccess = false, Message = ErrorMessages.RegistrationFailed};
            try
            {
                if (!ModelState.IsValid)
                {
                    return Ok(model);
                }

                if (CheckTokenValidity(input.Token))
                {
                    /*Calling register method from registration helper. */
                    RegistrationHelper.registrationHelper.RegisterMember(input.Model, HttpContext, _logger);
                    var members = JekyllHandler.MemberProvider.GetMembers();
                    //Checks, if it really is the first member, registration was successful and not some student used a loophole.
                    if (members.Count() == 1)
                    {
                        var member = members.ToDictionary(x => x.Id).Values.OrderBy(x => x.DateOfEntry).First();
                        JekyllHandler.MemberProvider.UpdateRoles(member, new[] {"admin", "creator"});
                        model.HasSuccess = true;
                        model.Message = SuccessMessages.RegistrationSucceeded;
                    }
                }
            }
            catch (Exception)
            {
                JekyllHandler.Log.Error("Incorrect input to RegisterFirstAdmin.");
            }

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpPost("CreateGlobalRanklist")]
        public IActionResult CreateGlobalRanklist([FromBody] GlobalRanklist ranklist)
        {
            return Ok(JekyllHandler.Domain.Interactions.CreateGlobalRankList(ranklist));
        }

        [AllowAnonymous]
        [HttpGet("RankingAlreadyExists")]
        public IActionResult CheckForGlobalRanklist()
        {
            var list = JekyllHandler.Domain.Query.GetGlobalRanklist();
            if (list.CurrentSemester == null)
            {
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
