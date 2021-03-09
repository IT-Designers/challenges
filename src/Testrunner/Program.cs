using System.Collections.Generic;
using SubmissionEvaluation.ChallengeEstimator;
using SubmissionEvaluation.Compilers;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;
using SubmissionEvaluation.Domain;
using SubmissionEvaluation.Providers;
using SubmissionEvaluation.Providers.FileProvider;
using SubmissionEvaluation.Providers.LogProvider;
using SubmissionEvaluation.Providers.MemberProvider;
using SubmissionEvaluation.Providers.ProcessProvider;

namespace Testrunner
{
    internal static class Program
    {
        private static void Main()
        {
            var Log = new Logger(null);
            var PathToData = @"x:\projects\work\itd\Challenges\web\";
            var PathToServerWwwRoot = @"x:\projects\work\itd\Challenges\wwwroot\";

            var fileProvider = new FileProvider(Log, PathToData, PathToServerWwwRoot, false);
            var memberProvider = new MemberProvider(Log, fileProvider, MemberType.Local, false);
            var processProvider = new ProcessProvider();
            var sandboxedProcessProvider = new DockerProcessProvider(Log);
            var challengeEstimator = new ChallengeEstimator();

            var compilers = new List<ICompiler> {new JavaMavenCompiler(Log, "java", "mvn"), new CsCompiler(Log, "dotnet")};

            var domain = new Domain(fileProvider, memberProvider, processProvider, sandboxedProcessProvider, challengeEstimator, null, compilers, Log, true,
                true, true, "", "");

            domain.Interactions.ProcessReview("li6hu3_10D0693AE3F963A7594F0CC8EEC07D76", "StayInContact4");
        }
    }
}
