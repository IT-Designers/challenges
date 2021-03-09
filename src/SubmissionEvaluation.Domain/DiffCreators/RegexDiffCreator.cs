using System.Text.RegularExpressions;
using System.Web;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal class RegexDiffCreator : IDiffCreator
    {
        private readonly bool ignoreCase;
        private readonly bool ignoreUmlauts;
        private readonly TrimMode trimMode;
        private readonly bool unifyFloatingNumbers;
        private readonly WhitespacesMode whitespacesMode;

        public RegexDiffCreator(TrimMode trimMode, bool ignoreCase, bool ignoreUmlauts, bool unifyFloatingNumbers, WhitespacesMode whitespacesMode)
        {
            this.trimMode = trimMode;
            this.ignoreCase = ignoreCase;
            this.ignoreUmlauts = ignoreUmlauts;
            this.unifyFloatingNumbers = unifyFloatingNumbers;
            this.whitespacesMode = whitespacesMode;
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
