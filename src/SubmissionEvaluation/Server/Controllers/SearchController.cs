using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;
using SubmissionEvaluation.Shared.Models;

namespace SubmissionEvaluation.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "IsChallengePlattformUser")]
    public class SearchController : Controller
    {
        [HttpGet("Results/{value}")]
        public IActionResult Get([FromRoute] string value)
        {
            var member = JekyllHandler.GetMemberForUser(User);
            var challenges = JekyllHandler.Domain.Query.GetAllChallenges(member);
            var bundles = JekyllHandler.Domain.Query.GetAllBundles(member).ToList();

            var searchResults = new List<SearchResult>();
            searchResults.AddRange(challenges.Select(p => new SearchResult {Text = p.Title, Url = $"/Challenge/View/{p.Id}"}));
            searchResults.AddRange(challenges.Select(p => new SearchResult {Text = p.Id, Url = $"/Challenge/View/{p.Id}"}));
            searchResults.AddRange(bundles.Select(p => new SearchResult {Text = p.Title, Url = $"/Bundle/View/{p.Id}"}));
            searchResults.AddRange(bundles.Select(p => new SearchResult {Text = p.Id, Url = $"/Bundle/View/{p.Id}"}));
            if (Settings.Features.EnableRating || User.IsInRole("admin"))
            {
                var members = JekyllHandler.MemberProvider.GetMembers().ToList();
                searchResults.AddRange(members.Select(p => new SearchResult {Text = p.Name, Url = $"/Members/Member/{p.Id}"}));
                searchResults.AddRange(members.Select(p => new SearchResult {Text = p.Id, Url = $"/Members/Member/{p.Id}"}));
            }

            var results = searchResults.Where(x => !string.IsNullOrWhiteSpace(x.Text)).Where(x => x.Text.ToLower().Contains(value.ToLower())).ToList();
            results.Sort((a, b) => a.Text.IndexOf(value) - b.Text.IndexOf(value));
            results = results.Take(15).ToList();
            return Ok(results);
        }
    }
}
