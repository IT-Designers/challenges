using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Achievements
{
    public interface IAchievementRater
    {
        List<Achievement> ListOfAchievements { get; }
        void AddAwards(Awards awards, AchievementConditions conditions);
    }
}
