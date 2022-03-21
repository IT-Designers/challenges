using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface IMemberProvider
    {
        IEnumerable<IMember> GetMembers();
        IMember GetMemberById(string id, bool returnMissingMember = false);
        IMember GetMemberByName(string name);
        IMember GetMemberByMail(string email);
        IMember GetMemberByUid(string uid);
        IMember AddMember(string name, string email, string uid = null, bool temporaryUser = false);
        void DeleteMember(IMember member);
        void DeleteAllSubmissionsByMember(IMember member);
        void IncreaseReviewFrequency(IMember member);
        void LogLastActivity(IMember member);
        void DecreaseReviewFrequency(IMember member);
        void UpdateLastReviewDate(IMember member);
        void ResetMemberAvailableChallenges(IMember member);
        void UpdateReviewLevel(IMember member, string language, ReviewLevelType level);
        void UpdateAllReviewLevelsAndCounters(IMember member, Dictionary<string, ReviewLevelAndCounter> model);
        void IncreaseReviewCounter(IMember member, string language);
        void AddReviewLanguage(IMember member, string language);
        void SetInactive(IMember member, bool inactive);
        void UpdateUid(IMember member, string uid);
        void UpdatePassword(IMember member, string password);
        void UpdateName(IMember member, string name);
        void UpdateMail(IMember member, string mail);
        void UpdateLastNotificationCheck(IMember member);
        void UpdateRoles(IMember member, string[] roles);
        void ActivatePendingMember(IMember member);
        void UpdateGroups(IMember member, string[] groups);
        void UpdateSolvedChallenges(IMember member, string[] solved);
        void UpdateCanRate(IMember member, string[] canRate);
        void UpdateUnlockedChallenges(IMember member, string[] unlocked);
        void UpdateAverageDifficultyLevel(IMember member, int avgDifficultyLevel);
    }
}
