using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Providers.MemberProvider
{
    public class MemberProvider : IMemberProvider
    {
        private readonly MemberType defaultMemberType;
        private readonly IFileProvider fileProvider;
        private readonly ILog log;
        private readonly Member removedMember = new Member {Id = Member.RemovedEntryId, Name = "xxx"};
        private readonly bool requiresMemberActivation;

        public MemberProvider(ILog log, IFileProvider fileProvider, MemberType defaultMemberType, bool requiresMemberActivation)
        {
            this.defaultMemberType = defaultMemberType;
            this.fileProvider = fileProvider;
            this.requiresMemberActivation = requiresMemberActivation;
            this.log = log;
        }

        public IMember AddMember(string name, string email, string uid = null, bool temporaryUser = false)
        {
            static string CreateUniqueId(string usedName)
            {
                using var md5 = new MD5CryptoServiceProvider();
                var md5Sum = md5.ComputeHash(Encoding.UTF32.GetBytes(usedName));
                return BitConverter.ToString(md5Sum).Replace("-", "").ToLowerInvariant();
            }

            uid = uid?.ToLowerInvariant();
            var member = new Member {Name = name, Mail = email, Type = temporaryUser ? MemberType.Local : defaultMemberType};
            if (temporaryUser)
            {
                member.State = MemberState.Temporary;
            }
            else
            {
                member.State = requiresMemberActivation ? MemberState.Pending : MemberState.Active;
            }

            member.DateOfEntry = member.LastActivity = DateTime.Today;

            var isFirstUser = !fileProvider.LoadMembers().Any();

            if (uid == null)
            {
                member.Id = CreateUniqueId(name);
            }
            else
            {
                member.Uid = member.Id = uid;
            }

            if (isFirstUser)
            {
                member.Roles = new[] {"admin"};
            }

            fileProvider.CreateMember(member);

            return member;
        }

        public void UpdateUid(IMember member, string uid)
        {
            if (member != null)
            {
                uid = uid?.ToLowerInvariant();
                var members = fileProvider.LoadMembers();
                using var @lock = fileProvider.GetLock();
                if (members.Any(x =>
                    string.Equals(x.Uid, uid, StringComparison.InvariantCultureIgnoreCase) &&
                    !string.Equals(x.Id, member.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new Exception("Uid already in use");
                }

                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.Uid = uid;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdatePassword(IMember member, string password)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadMember = fileProvider.LoadMember(member.Id, @lock);
                loadMember.Password = password;
                fileProvider.SaveMember(loadMember, @lock);
            }
        }

        public void UpdateName(IMember member, string name)
        {
            if (member != null)
            {
                var members = fileProvider.LoadMembers();
                using var @lock = fileProvider.GetLock();
                if (members.Any(x =>
                    string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase) &&
                    !string.Equals(x.Id, member.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new Exception("Name already in use");
                }

                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.Name = name;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateMail(IMember member, string mail)
        {
            if (member != null)
            {
                var members = fileProvider.LoadMembers();
                using var @lock = fileProvider.GetLock();
                if (members.Any(x =>
                    string.Equals(x.Mail, mail, StringComparison.InvariantCultureIgnoreCase) &&
                    !string.Equals(x.Id, member.Id, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new Exception("E-Mail already in use");
                }

                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.Mail = mail;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateLastNotificationCheck(IMember member)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.LastNotificationCheck = DateTime.Now;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateRoles(IMember member, string[] roles)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.Roles = roles;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void ResetMemberAvailableChallenges(IMember member)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.UnlockedChallenges = new string[]{};
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void ActivatePendingMember(IMember member)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.State = MemberState.Active;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateGroups(IMember member, string[] groups)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.Groups = groups;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateSolvedChallenges(IMember member, string[] solved)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.SolvedChallenges = solved;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateCanRate(IMember member, string[] canRate)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.CanRate = canRate;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateUnlockedChallenges(IMember member, string[] unlocked)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.UnlockedChallenges = unlocked;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateAverageDifficultyLevel(IMember member, int avgDifficultyLevel)
        {
            if (member != null && member.AverageDifficultyLevel != avgDifficultyLevel)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.AverageDifficultyLevel = avgDifficultyLevel;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void IncreaseReviewFrequency(IMember member)
        {
            if (member?.ReviewFrequency > 0)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.ReviewFrequency = loadedMember.ReviewFrequency+1;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void LogLastActivity(IMember member)
        {
            if (member != null)
            {
                if (member.LastActivity == DateTime.Today && member.State != MemberState.Inactive)
                {
                    return;
                }

                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.LastActivity = DateTime.Today;
                loadedMember.State = MemberState.Active;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void DecreaseReviewFrequency(IMember member)
        {
            if (member == removedMember)
            {
                log.Warning("Methode DecreaseReviewFrequency was called with a removedMember");
                return;
            }

            if (member?.ReviewFrequency < 60)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                if (member.ReviewFrequency < 3)
                {
                    loadedMember.ReviewFrequency = 3;
                }
                else if (member.ReviewFrequency < 7)
                {
                    loadedMember.ReviewFrequency = 7;
                }
                else if (member.ReviewFrequency < 14)
                {
                    loadedMember.ReviewFrequency = 14;
                }
                else if (member.ReviewFrequency < 30)
                {
                    loadedMember.ReviewFrequency = 30;
                }
                else
                {
                    loadedMember.ReviewFrequency += 30;
                }

                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateLastReviewDate(IMember member)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.LastReview = DateTime.Today;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public IEnumerable<IMember> GetMembers()
        {
            return fileProvider.LoadMembers();
        }

        public IMember GetMemberById(string id, bool returnMissingMember = false)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            if (id == Member.RemovedEntryId)
            {
                return removedMember;
            }

            try
            {
                return fileProvider.LoadMember(id);
            }
            catch (Exception)
            {
                if (returnMissingMember)
                {
                    return new Member {Name = "Missing", Id = id};
                }
            }

            return null;
        }

        public IMember GetMemberByName(string name)
        {
            var members = GetMembers();
            return members.SingleOrDefault(x => string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));
        }


        public IMember GetMemberByUid(string uid)
        {
            var members = GetMembers();
            return members.SingleOrDefault(x => string.Equals(x.Uid, uid, StringComparison.InvariantCultureIgnoreCase));
        }

        public IMember GetMemberByMail(string email)
        {
            var members = GetMembers();
            return members.SingleOrDefault(x => string.Equals(x.Mail, email, StringComparison.InvariantCultureIgnoreCase));
        }

        public void DeleteMember(IMember member)
        {
            using var @lock = fileProvider.GetLock();
            fileProvider.DeleteMember(member, @lock);
        }

        public void DeleteAllSubmissionsByMember(IMember member)
        {
            using var @lock = fileProvider.GetLock();
            var subs = fileProvider.LoadAllSubmissionsFor(member, true);
            foreach (var item in subs)
            {
                fileProvider.DeleteSubmission(item);
            }
        }

        public void UpdateReviewLevel(IMember member, string language, ReviewLevelType level)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.ReviewLanguages[language].ReviewLevel = level;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void UpdateAllReviewLevelsAndCounters(IMember member, Dictionary<string, ReviewLevelAndCounter> model)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                if (loadedMember.ReviewLanguages==null)
                {
                    loadedMember.ReviewLanguages = new Dictionary<string, ReviewLevelAndCounter>();
                }
                else
                {
                    loadedMember.ReviewLanguages.Clear();
                }
                foreach (var item in model)
                {
                    loadedMember.ReviewLanguages.Add(item.Key, new ReviewLevelAndCounter{ReviewCounter = item.Value.ReviewCounter, ReviewLevel = item.Value.ReviewLevel});
                }
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void IncreaseReviewCounter(IMember member, string language)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                if (loadedMember.ReviewLanguages.ContainsKey(language))
                    loadedMember.ReviewLanguages[language].ReviewCounter = loadedMember.ReviewLanguages[language].ReviewCounter+1;
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void AddReviewLanguage(IMember member, string language)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var loadedMember = fileProvider.LoadMember(member.Id, @lock);
                loadedMember.ReviewLanguages.Add(language, new ReviewLevelAndCounter{ ReviewLevel=ReviewLevelType.Beginner, ReviewCounter=0});
                fileProvider.SaveMember(loadedMember, @lock);
            }
        }

        public void SetInactive(IMember member, bool inactive)
        {
            if (member != null)
            {
                using var @lock = fileProvider.GetLock();
                var modMember = fileProvider.LoadMember(member.Id, @lock);
                if (!inactive && modMember.State == MemberState.Inactive)
                {
                    modMember.State = MemberState.Active;
                }

                if (inactive && modMember.State == MemberState.Active)
                {
                    modMember.State = MemberState.Inactive;
                }

                fileProvider.SaveMember(modMember, @lock);
            }
        }
    }
}
