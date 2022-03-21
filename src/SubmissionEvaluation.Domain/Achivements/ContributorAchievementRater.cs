using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Achivements
{
    public class ContributorAchievementRater : IAchievementRater
    {
        private const string ContributorAchievement = "contributor";

        public List<Achievement> ListOfAchievements => new List<Achievement> {new Achievement {Id = ContributorAchievement, Quality = QualityType.Gold}};

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            foreach (var contributor in conditions.Contributors)
            {
                if (contributor != null)
                {
                    awards.AwardWith(contributor, ContributorAchievement);
                }
            }
        }
    }
}
