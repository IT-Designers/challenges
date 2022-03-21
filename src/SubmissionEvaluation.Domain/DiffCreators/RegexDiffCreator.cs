using System.Text.RegularExpressions;
using System.Web;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal class RegexDiffCreator : IDiffCreator
    {
        private readonly bool ignoreCase;

        public RegexDiffCreator(bool ignoreCase)
        {
            this.ignoreCase = ignoreCase;
        }

        public (bool Success, string Details, bool showExpected) GetDiff(string submission, string solution)
        {
            var options = RegexOptions.Singleline;
            if (ignoreCase)
            {
                options |= RegexOptions.IgnoreCase;
            }

            var regex = new Regex(solution.Replace("\r\n", "\n"), options);
            var matches = regex.Matches(submission.Replace("\r\n", "\n"));
            if (matches.Count > 0)
            {
                return (true, "", true);
            }

            return (false, "<p>Die Ausgabe erfüllt nicht den angegeben Regulären Ausdruck:</p><pre><code>" + HttpUtility.HtmlEncode(solution) + "</code></pre>",
                true);
        }
    }
}
