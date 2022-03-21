using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Domain.Achivements;

namespace SubmissionEvaluationTest.Domain
{
    [TestFixture]
    public class TestContributorAchievementRater
    {
        [Test]
        public void AddAwards_Should_Add_Achievement_For_Contributor()
        {
            var rater = new ContributorAchievementRater();
            var awards = new Awards();
            var contributors = new List<string> {"123"};
            rater.AddAwards(awards, new AchievementConditions {Contributors = contributors});

            Assert.Contains(new Award {Id = "contributor"}, awards["123"].ToList());
        }
    }
}
