using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Domain.Achievements;

namespace SubmissionEvaluationTest.Domain
{
    [TestFixture]
    public class TestChallengeAuthorAchievementRater
    {
        [Test]
        public void AddAwards_Should_Add_Achievement_For_First_Submission()
        {
            var rater = new ChallengeAuthorAchievementRater();
            var awards = new Awards();
            var challengeAuthors = new List<string>
            {
                "123",
                "123",
                "123",
                "123",
                "123",
                "245"
            };
            rater.AddAwards(awards, new AchievementConditions {ChallengeAuthors = challengeAuthors});

            Assert.Contains(new Award {Id = "writer"}, awards["123"].ToList());
            Assert.Contains(new Award {Id = "selfpublisher"}, awards["123"].ToList());
            Assert.Contains(new Award {Id = "selfpublisher"}, awards["245"].ToList());
        }
    }
}
