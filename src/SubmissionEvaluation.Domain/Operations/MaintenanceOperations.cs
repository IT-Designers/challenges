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
                var author = MemberProvider.GetMemberById(challenge.AuthorId);
                if (author != null && challenge.IsAvailable && author.Id == member.Id)
                {
                    throw new Exception("Member is author of challenge " + challenge.Id);
                }
            }
        }

        public void Cleanup()
        {
            var members = MemberProvider.GetMembers();
            foreach (var member in members)
            {
                try
                {
                    if (DateTime.Now - member.LastActivity > TimeSpan.FromDays(365))
                    {
                        MemberProvider.DeleteAllSubmissionsByMember(member);
                        MemberProvider.DeleteMember(member);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e, "{User} auf inaktiv setzen fehlgeschlagen.", member.Name);
                }
            }
        }

        public void MarkActivityStatusForUsers()
        {
            var members = MemberProvider.GetMembers();
            foreach (var member in members)
            {
                try
                {
                    if (DateTime.Now - member.LastActivity > TimeSpan.FromDays(180))
                    {
                        MemberProvider.SetInactive(member, true);
                    }
                    else
                    {
                        MemberProvider.SetInactive(member, false);
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
                updated.AuthorId = newId;
                updated.LastEditorId = null;
                updated.IsDraft = true;
                ProviderStore.FileProvider.SaveChallenge(updated, writeLock);
            }
        }
    }
}
