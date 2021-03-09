using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;

namespace SubmissionEvaluation.Domain.Achievements
{
    public class AchievementConditions
    {
        public GlobalRanklist GlobalRanklist { get; internal set; }
        public IEnumerable<GlobalRanklist> SemesterRanklists { get; internal set; }
        public IEnumerable<SubmitterRankings> Rankings { get; internal set; }
        public IEnumerable<SubmitterHistory> Histories { get; internal set; }
        public IEnumerable<string> Contributors { get; internal set; }
        public IEnumerable<Challenge> Challenges { get; internal set; }
        public IEnumerable<string> ChallengeAuthors { get; internal set; }
        public IEnumerable<ChallengeRanklist> ChallengeRanklists { get; internal set; }
        public IEnumerable<IMember> Members { get; internal set; }
        public IEnumerable<string> CompilerNames { get; internal set; }
    }
}
