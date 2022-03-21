using System;
using System.Threading;
using Hangfire;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Providers.LogProvider;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server
{
    public class SchedulesAndTasks
    {
        public static void Schedule_AchievementsUpdate()
        {
            JekyllHandler.Domain.Interactions.UpdateAchievements();
        }

        public static void Schedule_RankingsUpdate()
        {
            JekyllHandler.Domain.Interactions.UpdateChallengeRankings();
        }

        public static void Schedule_ChallengeStatsUpdate()
        {
            JekyllHandler.Domain.Interactions.UpdateChallengeEstimation();
            JekyllHandler.Domain.Interactions.UpdateChallengeDifficultyRatings();
            JekyllHandler.Domain.Interactions.UpdateCategoryPages();
        }

        public static void Schedule_Cleanup()
        {
            JekyllHandler.Domain.Interactions.IdentifyActivityStatusForMembers();
            if (Settings.Application.DeleteAfterMoreThenOneYearInactivity)
            {
                JekyllHandler.Domain.Interactions.Cleanup();
            }
        }

        public static void Schedule_PromotionsAndMails()
        {
            JekyllHandler.Domain.Interactions.PromoteNewReviewers();
            ((Logger) JekyllHandler.Log).SendDelayedErrorReports();
        }

        public static void Schedule_PendingReviews()
        {
            JekyllHandler.Domain.Interactions.ProcessPendingReviews();
            JekyllHandler.Domain.Interactions.MarkOldReviews();
            JekyllHandler.Domain.Interactions.StartNewReviews();
        }

        public static void EnqueueUnprocessedSubmission(ISubmission x)
        {
            Console.WriteLine("Processing submission " + x.Challenge + " from " + x.MemberName);
            BackgroundJob.Enqueue(() => Task_ProcessSubmission(x.SubmissionId, x.Challenge));
        }

        [QueueUniqueItemFilter]
        public static void Task_ProcessSubmission(string id, string challenge)
        {
            JekyllHandler.Domain.Interactions.ProcessSubmission(id, challenge);
        }

        [QueueUniqueItemFilter]
        public static void Task_ProcessReview(string id, string challenge)
        {
            JekyllHandler.Domain.Interactions.ProcessReview(id, challenge);
        }

        public static void Schedule_CheckForUnprocessedSubmissions()
        {
            JekyllHandler.Domain.Interactions.CheckForUnprocessedSubmissions(EnqueueUnprocessedSubmission);
        }

        public static void EnqueueUnprocessedReview(ISubmission x)
        {
            BackgroundJob.Enqueue(() => Task_ProcessReview(x.SubmissionId, x.Challenge));
        }

        public static void EnableMaintenanceMode()
        {
            WaitTillNoOpenJobs();
            JekyllHandler.Domain.Interactions.EnableMaintenanceMode();
        }

        private static void WaitTillNoOpenJobs()
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();

            long GetOpenJobCount()
            {
                var runningJobs = monitoringApi.ProcessingCount();
                var defaultJobs = monitoringApi.EnqueuedCount("default");
                return runningJobs + defaultJobs;
            }

            while (GetOpenJobCount() > 0)
            {
                Thread.Sleep(5000);
            }
        }

        public static void DisableMaintenanceMode()
        {
            JekyllHandler.Domain.Interactions.DisableMaintenanceMode();
        }

        public static void Shutdown()
        {
            JekyllHandler.Log.Warning("Fahre system herunter");
            WaitTillNoOpenJobs();

            Program.Host.StopAsync().Wait();
            Environment.Exit(0);
        }

        public static void Schedule_DuplicateCheck()
        {
            JekyllHandler.Domain.Interactions.AnalyseCodeDuplicates();
        }

        public static void Schedule_UnlockChallenges()
        {
            JekyllHandler.Domain.Interactions.UnlockChallengesForMembers();
        }
    }
}
