using System;
using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.Operations
{
    internal static class SubmissionOperations
    {
        public static ISubmission SaveNewSubmission(ProviderStore providerStore, IMember member, DateTime date, byte[] zipData, string challengeName,
            ICompiler compiler)
        {
            var files = providerStore.FileProvider.GetFilenamesInsideZip(zipData);
            var compilableFiles = compiler.GetSourceFiles(files);
            var result = providerStore.FileProvider.StoreNewSubmission(member, date, challengeName, zipData, compilableFiles);
            var challenge = providerStore.FileProvider.LoadChallenge(challengeName);
            if (challenge.IsAvailable)
            {
                providerStore.Log.Activity(member.Id, ActivityType.NewSubmission, challengeName);
            }

            return result;
        }

        internal static void DeleteSubmissionForMember(ProviderStore providerStore, IMember member)
        {
            var allSubmissions = providerStore.FileProvider.LoadAllSubmissions(true).ToList();
            foreach (var submission in allSubmissions.Where(x => x.MemberId == member.Id))
            {
                DeleteSubmission(submission, allSubmissions, providerStore.FileProvider);
            }
        }

        private static void DeleteSubmission(Result submission, List<Result> allSubmissions, IFileProvider fileProvider)
        {
            fileProvider.DeleteSubmission(submission);
            foreach (var check in allSubmissions)
            {
                if (check.DuplicateId == submission.SubmissionId)
                {
                    fileProvider.SetDuplicateScore(check, null, null);
                }
            }
        }

        public static void DeleteChallengeSubmissions(IChallenge challenge, IFileProvider fileProvider)
        {
            var allSubmissions = fileProvider.LoadAllSubmissionsFor(challenge, true).ToList();
            foreach (var submission in allSubmissions)
            {
                DeleteSubmission(submission, allSubmissions, fileProvider);
            }
        }
    }
}
