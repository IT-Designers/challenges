using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Domain.Properties;

namespace SubmissionEvaluation.Domain
{
    public static class MailMessages
    {
        public static string CreateReviewMailBody(IMember member, Result result, int outstandingReviews, int dueDays, string siteUrl, string helpEmail)
        {
            return string.Format(Resources.MailMessages_ReviewBody, member.FirstName, outstandingReviews, result.SizeInBytes, dueDays,
                $"{siteUrl}/Review/Overview") + string.Format(Resources.MailMessages_Footer, siteUrl, helpEmail);
        }

        public static string CreateReviewSubject(Result result)
        {
            return string.Format(Resources.MailMessages_ReviewHeader, result.Challenge, result.Language);
        }

        public static string CreateInactivityReminderUnsolvedChallenge(IMember member, Challenge challenge, string siteUrl, string helpEmail)
        {
            return string.Format(Resources.MailMessages_InactivityReminderUnsolvedChallenge, member.FirstName, challenge.Title,
                $"{siteUrl}/challenges/{challenge.Id}/challenge.html") + string.Format(Resources.MailMessages_Footer, siteUrl, helpEmail);
        }

        public static string CreateInactivityReminderChallengeRecommendation(IMember member, Challenge challenge, string siteUrl, string helpEmail)
        {
            return string.Format(Resources.MailMessages_InactivityReminderChallengeRecommendation, member.FirstName, challenge.Title,
                $"{siteUrl}/challenges/{challenge.Id}/challenge.html") + string.Format(Resources.MailMessages_Footer, siteUrl, helpEmail);
        }

        public static string CreateInactivityReminderHeader()
        {
            return "Seit 6 Monaten inaktiv im Challenges Portal";
        }
    }
}
