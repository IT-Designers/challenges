using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Review;
using System;

namespace SubmissionEvaluation.Domain.Achivements
{
    internal class ReviewAchievementRater : IAchievementRater
    {
        private const string Coach = "coach";
        private const string Teacher = "teacher";
        private const string TeachingMaster = "teachingMaster";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = Coach, Quality = QualityType.Bronze},
                new Achievement {Id = Teacher, Quality = QualityType.Silver},
                new Achievement {Id = TeachingMaster, Quality = QualityType.Gold}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            foreach (var member in conditions.Members)
            {
                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevelType.Intermediate))
                {
                    awards.AwardWith(member.Id, Coach);
                }

                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevelType.Expert))
                {
                    awards.AwardWith(member.Id, Teacher);
                }

                if (IsReviewLevelEqualOrHigherAndActiveReviewer(member, ReviewLevelType.Master))
                {
                    awards.AwardWith(member.Id, TeachingMaster);
                }
            }
        }

        private bool IsReviewLevelEqualOrHigherAndActiveReviewer(IMember member, ReviewLevelType compare)
        {
            if (member.ReviewLanguages != null && member.ReviewLanguages.Count > 0)
            {
                foreach (KeyValuePair<string, ReviewLevelAndCounter> item in member.ReviewLanguages)
                {
                    if (item.Value.ReviewLevel >= compare && item.Value.ReviewLevel != ReviewLevelType.Deactivated && item.Value.ReviewLevel != ReviewLevelType.Deactivated) return true;
                }
            }
            return false;
        }
    }
}
