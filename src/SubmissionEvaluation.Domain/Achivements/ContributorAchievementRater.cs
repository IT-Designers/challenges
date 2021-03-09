using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Achievements
{
    public class ContributorAchievementRater : IAchievementRater
    {
        private const string contributorAchievement = "contributor";

        public List<Achievement> ListOfAchievements => new List<Achievement> {new Achievement {Id = contributorAchievement, Quality = QualityType.Gold}};

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            foreach (var contributor in conditions.Contributors)
            {
                if (contributor != null)
                {
                    awards.AwardWith(contributor, contributorAchievement);
                }
            }
        }
    }
}
