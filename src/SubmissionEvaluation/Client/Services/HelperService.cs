using System.Collections.Generic;

namespace SubmissionEvaluation.Client.Services
{
    public class HelperService
    {
        public string CalculateDifficultyColor(int? difficultyRating)
        {
            if (difficultyRating == null)
            {
                return "#666";
            }

            var r = difficultyRating <= 50 ? 13 * difficultyRating / 50.0f : 13;
            var g = difficultyRating <= 50 ? 13 : 13 - 13 * (difficultyRating - 50) / 50.0f;
            var color = $"#{(int) r + 2:X}{(int) g + 2:X}0";
            return color;
        }

        public void MoveChallengeUp(List<string> challenges, string challenge)
        {
            var index = challenges.IndexOf(challenge);
            if (index > 0)
            {
                challenges.RemoveAt(index);
                challenges.Insert(index - 1, challenge);
            }
        }

        public void MoveChallengeDown(List<string> challenges, string challenge)
        {
            var index = challenges.IndexOf(challenge);
            if (index >= 0)
            {
                challenges.RemoveAt(index);
                challenges.Insert(index + 1, challenge);
            }
        }
    }
}
