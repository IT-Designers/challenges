using System.Collections.Generic;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentMatchEntry
    {
        public TournamentEntry Author { get; set; }

        public string TargetDir => $"/testrun/bin/{Author.Id}";

        public IEnumerable<string> MatchRunnerLine
        {
            get
            {
                if (Author.StartArguments?.Length > 0)
                {
                    yield return $"\"{Author.StartCommand} {string.Join(" ", Author.StartArguments)}\"".Replace("/testrun/bin", TargetDir);
                }
                else
                {
                    yield return Author.StartCommand.Replace("/testrun/bin", TargetDir);
                }

                yield return Author.WorkinDirectory.Replace("/testrun/bin", TargetDir);
            }
        }
    }
}
