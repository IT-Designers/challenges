using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Providers.CryptographyProvider;
using SubmissionEvaluation.Server.Classes.Authentication;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Server.Classes.Messages;
using SubmissionEvaluation.Shared.Classes;
using SubmissionEvaluation.Shared.Models;
using SubmissionEvaluation.Shared.Models.Account;
using SubmissionEvaluation.Shared.Models.Admin;
using SubmissionEvaluation.Shared.Models.Help;
using Member = SubmissionEvaluation.Contracts.ClientPocos.Member;

namespace SubmissionEvaluation.Server.Controllers
{
    [Authorize(Policy = "IsChallengePlattformUser")]
    [Authorize(Roles = "admin,groupAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly ILogger logger;

        public AdminController(ILogger<AdminController> logger)
        {
            this.logger = logger;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("Users")]
        public IActionResult Users()
        {
            return Ok(FetchBasicModel());
        }

        private AdminUserModel<Member> FetchBasicModel()
        {
            var members = JekyllHandler.MemberProvider.GetMembers();
            var model = new AdminUserModel<Member> { Members = members.Select(x => new Member(x)).ToList() };
            return model;
        }

        [Authorize(Roles = "admin")]
        [HttpPost("AddUser")]
        public IActionResult AddUser([FromBody] AddTempUserModel model)
        {
            var pwdHash = CryptographyProvider.CreateArgon2Password(model.Password);
            var member = JekyllHandler.Domain.Interactions.AddMember(model.Name, model.Mail, model.Name, true);
            JekyllHandler.MemberProvider.UpdatePassword(member, pwdHash);
            return Ok(new GenericModel { Message = SuccessMessages.GenericSuccess, HasSuccess = true });
        }

        [HttpGet("ExportMembers")]
        public IActionResult ExportMembers()
        {
            try
            {
                var members = JekyllHandler.MemberProvider.GetMembers();
                var memberLines = members.Select(x => $"{x.Name};{x.Mail};{x.DateOfEntry.ToShortDateString()}");
                var header = "Name;Mail;Datum";
                var txtData = header + Environment.NewLine + string.Join(Environment.NewLine, memberLines);
                var data = Encoding.UTF8.GetBytes(txtData);
                return Ok(new DownloadInfo(data));
            }
            catch
            {
                return Ok(new DownloadInfo(ErrorMessages.GenericError));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("ExportSolutions")]
        public IActionResult ExportSolutions()
        {
            try
            {
                var data = JekyllHandler.Domain.Query.ExportSolutionsAsZip();
                return Ok(new DownloadInfo(data));
            }
            catch (Exception ex)
            {
                JekyllHandler.Log.Error(ex, "Erstellen des Sources.zip fehlgeschlagen");
                return Ok(new DownloadInfo(ErrorMessages.GenericError));
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("DisableMaintenanceMode")]
        public IActionResult DisableMaintenanceMode()
        {
            JekyllHandler.Domain.Interactions.DisableMaintenanceMode();
            return Ok(JekyllHandler.Domain.IsMaintenanceMode);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("EnableMaintenanceMode")]
        public IActionResult EnableMaintenanceMode()
        {
            JekyllHandler.Domain.Interactions.EnableMaintenanceMode();
            return Ok(JekyllHandler.Domain.IsMaintenanceMode);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("DeleteMember")]
        public IActionResult DeleteMember([FromBody] string id)
        {
            JekyllHandler.Domain.Interactions.DeleteMember(id);
            return Ok(new GenericModel { HasSuccess = true, Message = SuccessMessages.GenericSuccess });
        }

        [HttpPost("ResetMemberAvailableChallenges")]
        public IActionResult ResetMemberAvailableChallenges([FromBody] string id)
        {
            var currentMember = JekyllHandler.GetMemberForUser(User);
            if (!(currentMember.IsAdmin || IsMemberInManagedGroup(id)))
            {
                return Ok(new GenericModel { HasError = true, Message = ErrorMessages.NoPermission });
            }

            JekyllHandler.Domain.Interactions.ResetMemberAvailableChallenges(id);
            var model = FetchBasicModel();
            model.Message = SuccessMessages.GenericSuccess;
            model.HasSuccess = true;
            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("ActivatePendingMember")]
        public IActionResult ActivatePendingMember([FromBody] string id)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            JekyllHandler.Domain.Interactions.ActivatePendingMember(member);
            var model = FetchBasicModel();
            model.Message = SuccessMessages.GenericSuccess;
            model.HasSuccess = true;
            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("EditHelpPage")]
        public IActionResult EditHelpPage([FromBody] string path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                {
                    return Ok(new GenericModel { HasError = true, Message = ErrorMessages.IdError });
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
                    Description = helpPage.Description,
                    Title = helpPage.Title,
                    Parent = helpPage.Parent,
                    Pages = helpHierarchie
                });
            }
            catch (Exception)
            {
                return Ok(new GenericModel { HasError = true, Message = ErrorMessages.IdError });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("UpdateHelpPage")]
        public IActionResult EditHelpPage(HelpModel model)
        {
            model.Pages = JekyllHandler.Domain.Query.GetHelpHierarchy();
            if (!ModelState.IsValid)
            {
                return Ok(model);
            }

            try
            {
                JekyllHandler.Domain.Interactions.EditHelpPage(new HelpPage
                {
                    Path = model.Path, Title = model.Title, Parent = model.Parent, Description = model.Description
                });
                model.HasError = false;
                model.HasSuccess = true;
                model.Message = SuccessMessages.EditBundle;

                return Ok(model);
            }
            catch (IOException ex)
            {
                logger.LogWarning(ex.Message);
                model.HasError = true;
                model.Message = ex.Message;
                return Ok(model);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost("ImpersonateMember")]
        public IActionResult ImpersonateMember([FromBody] string id)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            IdentityAuthenticator.LoginForMember(member, "Impersonate", HttpContext).WaitAndUnwrap(5000);
            return Ok(new GenericModel { Message = SuccessMessages.GenericSuccess, HasSuccess = true });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("AllPossibleMemberRoles/{id}")]
        public IActionResult ManageMemberRoles([FromRoute] string id)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            var groups = new[] { "admin", "creator", "reviewer", "groupAdmin" };
            var model = new ManageMemberRolesModel
            {
                Roles = groups.Select(x => new GroupInfo { Title = x, Id = x, Selected = member.Roles.Any(y => y == x) }).ToArray(),
                Name = member.Name,
                Id = id
            };

            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("NewMemberRoles")]
        public IActionResult ManageMemberRoles([FromBody] ManageMemberRolesModel model)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(model.Id);
            JekyllHandler.MemberProvider.UpdateRoles(member, model.Roles.Where(x => x.Selected).Select(x => x.Id).ToArray());
            return Ok(new GenericModel { HasSuccess = true, Message = SuccessMessages.GenericSuccess });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("AllPossibleMemberGroups/{id}")]
        public IActionResult ManageMemberGroups([FromRoute] string id)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            var groups = JekyllHandler.Domain.Query.GetAllGroups();
            var subgroups = groups.SelectMany(x => x.SubGroups);
            var model = new ManageMemberGroupsModel
            {
                Groups = AccountController.GetGroups(groups.Where(x => !subgroups.Contains(x.Id)).ToList(), member), Name = member.Name, Id = id
            };

            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("AllPossibleCompilers")]
        public IActionResult AllPossibleCompilers()
        {
            var compilers = JekyllHandler.Domain.Compilers;
            var temp_comp = new List<ReviewModel.Compiler>();
            foreach (var item in compilers)
            {
                temp_comp.Add(new ReviewModel.Compiler { Name = item.Name, Selected = false });
            }

            var model = new ReviewModel { Compilers = temp_comp };

            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("NewMemberGroups")]
        public IActionResult ManageMemberGroups([FromBody] ManageMemberGroupsModel model)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(model.Id);
            var groupsSelected = AccountController.UpdateGroupsSelected(model.Groups.Where(x => x.Selected));
            JekyllHandler.MemberProvider.UpdateGroups(member, groupsSelected);
            return Ok(new GenericModel { HasSuccess = true, Message = SuccessMessages.GenericSuccess });
        }

        [Authorize(Roles = "admin")]
        [HttpGet("GetAllReviewLevelForMember/{id}")]
        public IActionResult GetAllReviewLevelForMember([FromRoute] string id)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            var model = new ReviewerModel();
            model.ReviewLanguages = new Dictionary<string, ReviewLevelAndCounter>();
            var compilers = JekyllHandler.Domain.Compilers;
            foreach (var item in compilers)
            {
                model.ReviewLanguages.Add(item.Name, new ReviewLevelAndCounter { Selected = false });
            }

            if (member.ReviewLanguages != null)
            {
                foreach (var item in member.ReviewLanguages)
                {
                    if (!model.ReviewLanguages.ContainsKey(item.Key)) continue;
                    model.ReviewLanguages[item.Key].Selected = true;
                    model.ReviewLanguages[item.Key].ReviewLevel = item.Value.ReviewLevel;
                    model.ReviewLanguages[item.Key].ReviewCounter = item.Value.ReviewCounter;
                }
            }

            return Ok(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost("SetAllReviewLevelForMember/{id}")]
        public IActionResult SetAllReviewLevelForMember([FromRoute] string id, [FromBody] ReviewerModel data)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            var model = new Dictionary<string, ReviewLevelAndCounter>();
            if (data.ReviewLanguages == null) data.ReviewLanguages = new Dictionary<string, ReviewLevelAndCounter>();
            foreach (var item in data.ReviewLanguages)
            {
                if (item.Value.Selected) model.Add(item.Key, item.Value);
            }

            JekyllHandler.MemberProvider.UpdateAllReviewLevelsAndCounters(member, model);
            return Ok(new GenericModel { HasSuccess = true, Message = SuccessMessages.GenericSuccess });
        }

        [Authorize(Roles = "admin")]
        [HttpPost("IncreaseMemberReviewLevel/{language}")]
        public IActionResult IncreaseMemberReviewLevel([FromBody] string id, [FromRoute] string language)
        {
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            JekyllHandler.Domain.Interactions.IncreaseReviewLevel(member, language);
            var model = FetchBasicModel();
            model.HasSuccess = true;
            model.Message = SuccessMessages.GenericSuccess;
            return Ok(model);
        }

        [HttpPost("ResetMemberPassword")]
        public IActionResult ResetMemberPassword([FromBody] string id)
        {
            var currentMember = JekyllHandler.GetMemberForUser(User);
            if (!(currentMember.IsAdmin || IsMemberInManagedGroup(id)))
            {
                return Ok(new GenericModel { HasError = true, Message = ErrorMessages.NoPermission });
            }

            Func<string> passwordGenerator = () =>
            {
                var charSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!#$%&*@".ToArray();
                var stringBuilder = new StringBuilder();
                var random = new Random();
                for (var i = 0; i < 24; i++)
                {
                    stringBuilder.Append(charSet.ElementAt(random.Next(1, charSet.Length) - 1));
                }

                return stringBuilder.ToString();
            };

            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            var newPwd = passwordGenerator.Invoke();
            var pwdHash = CryptographyProvider.CreateArgon2Password(newPwd);
            JekyllHandler.MemberProvider.UpdatePassword(member, pwdHash);
            var model = new ResetPasswordModel<IMember> { Member = member, Password = newPwd };
            return Ok(model);
        }

        private bool IsMemberInManagedGroup(string id)
        {
            var currentMember = JekyllHandler.GetMemberForUser(User);
            var permissions = JekyllHandler.GetPermissionsForMember(currentMember);
            var managedGroupIds = JekyllHandler.Domain.Query.GetAllGroups().Where(x => permissions.GroupsAccessible.Contains(x.Id)).Select(x => x.Id);
            var member = JekyllHandler.MemberProvider.GetMemberById(id);
            return member.Groups.Intersect(managedGroupIds, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}
