using System;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.Operations
{
    internal class MaintenanceOperations
    {
        public ProviderStore ProviderStore { get; set; }
        public ILog Log { get; set; }
        public IMemberProvider MemberProvider { get; set; }
        public ISmtpProvider SmtpProvider { get; set; }

        public void EnsureMemberIsNotChallengeAuthor(IMember member)
        {
            var challenges = ProviderStore.FileProvider.LoadChallenges();
            foreach (var challenge in challenges)
            {
                var author = MemberProvider.GetMemberById(challenge.AuthorID);
                if (author != null && challenge.IsAvailable && author.Id == member.Id)
                {
                    throw new Exception("Member is author of challenge " + challenge.Id);
                }
            }
        }

        public void MarkInactiveUsers()
        {
            var members = MemberProvider.GetMembers();
            foreach (var member in members.Where(x => x.State == MemberState.Active))
            {
                try
                {
                    if (DateTime.Now - member.LastActivity > TimeSpan.FromDays(180))
                    {
                        MemberProvider.SetInactive(member, true);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "{User} auf inaktiv setzen fehlgeschlagen.", member.Name);
                }
            }
        }

        public void ResetChallengesAndFixId(string newId)
        {
            foreach (var props in ProviderStore.FileProvider.GetChallengeIds())
            {
                using var writeLock = ProviderStore.FileProvider.GetLock();
                var updated = ProviderStore.FileProvider.LoadChallenge(props, writeLock);
                updated.AuthorID = newId;
                updated.LastEditorID = null;
                updated.IsDraft = true;
                ProviderStore.FileProvider.SaveChallenge(updated, writeLock);
            }
        }
    }
}
