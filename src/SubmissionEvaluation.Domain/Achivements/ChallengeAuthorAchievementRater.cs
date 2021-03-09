using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Achievements
{
    public class ChallengeAuthorAchievementRater : IAchievementRater
    {
        private const string singleAchievement = "selfpublisher";
        private const string fiveAchievement = "writer";
        private const string tenAchievement = "kingOfAmazon";
        private const string fiftyAchievement = "nobelLaureate";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = singleAchievement, Quality = QualityType.Bronze},
                new Achievement {Id = fiveAchievement, Quality = QualityType.Silver},
                new Achievement {Id = tenAchievement, Quality = QualityType.Gold},
                new Achievement {Id = fiftyAchievement, Quality = QualityType.Platin}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            var grouped = conditions.ChallengeAuthors.GroupBy(x => x);
            foreach (var author in grouped)
            {
                var count = author.Count();
                awards.AwardWith(author.Key, singleAchievement);
                if (count >= 5)
                {
                    awards.AwardWith(author.Key, fiveAchievement);
                }

                if (count >= 25)
                {
                    awards.AwardWith(author.Key, tenAchievement);
                }

                if (count >= 50)
                {
                    awards.AwardWith(author.Key, fiftyAchievement);
                }
            }
        }
    }
}
