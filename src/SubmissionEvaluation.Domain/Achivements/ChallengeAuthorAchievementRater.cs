using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Achivements
{
    public class ChallengeAuthorAchievementRater : IAchievementRater
    {
        private const string SingleAchievement = "selfpublisher";
        private const string FiveAchievement = "writer";
        private const string TenAchievement = "kingOfAmazon";
        private const string FiftyAchievement = "nobelLaureate";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = SingleAchievement, Quality = QualityType.Bronze},
                new Achievement {Id = FiveAchievement, Quality = QualityType.Silver},
                new Achievement {Id = TenAchievement, Quality = QualityType.Gold},
                new Achievement {Id = FiftyAchievement, Quality = QualityType.Platin}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            var grouped = conditions.ChallengeAuthors.GroupBy(x => x);
            foreach (var author in grouped)
            {
                var count = author.Count();
                awards.AwardWith(author.Key, SingleAchievement);
                if (count >= 5)
                {
                    awards.AwardWith(author.Key, FiveAchievement);
                }

                if (count >= 25)
                {
                    awards.AwardWith(author.Key, TenAchievement);
                }

                if (count >= 50)
                {
                    awards.AwardWith(author.Key, FiftyAchievement);
                }
            }
        }
    }
}
