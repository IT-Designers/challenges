using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server
{
    public static class Program
    {
        public static IWebHost Host;

        public static int Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            SettingsParser.InitializeSettings(configuration["DataPath"]);
            if (Settings.Application.Inactive)
            {
                Console.WriteLine("Anwendungsinstanz derzeit inaktiv. Exit!");
                return 1;
            }

            var logStatusChanges = args.Length > 0 && args[0] == "rerun";

            JekyllHandler.Initialize(logStatusChanges);
            JekyllHandler.Domain.Interactions.NewSubmission += SchedulesAndTasks.EnqueueUnprocessedSubmission;
            JekyllHandler.Domain.Interactions.NewReview += SchedulesAndTasks.EnqueueUnprocessedReview;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "reset":
                        JekyllHandler.Domain.Interactions.ResetSystem(args.Skip(1).ToList());
                        return 0;
                    case "rerun":
                        if (args[1] == "failed")
                        {
                            JekyllHandler.Domain.Interactions.RerunFailedSubmissions();
                        }
                        else if (args[1] == "broken")
                        {
                            JekyllHandler.Domain.Interactions.RerunBrokenSubmissions();
                        }
                        else if (args[1] == "all")
                        {
                            JekyllHandler.Domain.Interactions.RerunSubmissions();
                        }
                        else if (args[1] == "working")
                        {
                            JekyllHandler.Domain.Interactions.RerunWorkingSubmissions();
                        }
                        else if (args.Length == 2)
                        {
                            JekyllHandler.Domain.Interactions.RerunSubmission(args[1]);
                        }

                        return 0;
                }
            }

            CheckAndLogIfNewVersion();
            JekyllHandler.Domain.Interactions.Migrate();
            JekyllHandler.Domain.Interactions.VerifyMembers();
            Host = CreateWebHost(args);
            Host.Run();
            return 0;
        }

        public static IWebHost CreateWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build()).UseStartup<Startup>().Build();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Unhandled Exception " + e.ExceptionObject);
        }

        private static void CheckAndLogIfNewVersion()
        {
            var currentVersionHash = Domain.Domain.GetVersionHash();

            if (JekyllHandler.Domain.Query.GetLastVersionHash() != currentVersionHash)
            {
                JekyllHandler.Log.Activity(null, ActivityType.VersionUpdate, currentVersionHash);
                JekyllHandler.Domain.Interactions.SaveLastVersionHash(currentVersionHash);
            }
        }
    }
}
