using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Domain.Achievements
{
    internal class ReviewAchievementRater : IAchievementRater
    {
        private const string coach = "coach";
        private const string teacher = "teacher";
        private const string teachingMaster = "teachingMaster";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = coach, Quality = QualityType.Bronze},
                new Achievement {Id = teacher, Quality = QualityType.Silver},
                new Achievement {Id = teachingMaster, Quality = QualityType.Gold}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            foreach (var member in conditions.Members)
            {
                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevel.Intermediate))
                {
                    awards.AwardWith(member.Id, coach);
                }

                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevel.Expert))
                {
                    awards.AwardWith(member.Id, teacher);
                }

                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevel.Master))
                {
                    awards.AwardWith(member.Id, teachingMaster);
                }
            }
        }

        private bool IsReviewLevelEqualOrHigherAndActiveReviewer(IMember member, ReviewLevel compare)
        {
            return member.ReviewLevel >= compare && member.ReviewLevel != ReviewLevel.Deactivated && member.ReviewLevel != ReviewLevel.Deactivated;
        }
    }
}
