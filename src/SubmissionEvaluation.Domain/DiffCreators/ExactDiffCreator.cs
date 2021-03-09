using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal class ExactDiffCreator : IDiffCreator
    {
        private readonly bool ignoreCase;
        private readonly bool ignoreUmlauts;
        private readonly bool matchSubstring;
        private readonly TrimMode trimMode;
        private readonly bool unifyFloatingNumbers;
        private readonly WhitespacesMode whitespacesMode;

        public ExactDiffCreator(TrimMode trimMode, bool ignoreCase, bool ignoreUmlauts, bool unifyFloatingNumbers, WhitespacesMode whitespacesMode,
            bool matchSubstring)
        {
            this.trimMode = trimMode;
            this.ignoreCase = ignoreCase;
            this.ignoreUmlauts = ignoreUmlauts;
            this.unifyFloatingNumbers = unifyFloatingNumbers;
            this.whitespacesMode = whitespacesMode;
            this.matchSubstring = matchSubstring;
        }

        public (bool Success, string Details, bool showExpected) GetDiff(string submission, string solution)
        {
            var arSubmissionLines = whitespacesMode == WhitespacesMode.RemoveEvenNewlines ? new[] {submission} : Regex.Split(submission.Trim(), "\r\n|\n");
            var arSolutionLines = whitespacesMode == WhitespacesMode.RemoveEvenNewlines ? new[] {solution} : Regex.Split(solution.Trim(), "\r\n|\n");

            var submissionText = string.Join(Environment.NewLine,
                arSubmissionLines.Select(x => DiffHelper.CleanString(x, trimMode, ignoreCase, ignoreUmlauts, unifyFloatingNumbers, whitespacesMode)));
            var solutionText = string.Join(Environment.NewLine,
                arSolutionLines.Select(x => DiffHelper.CleanString(x, trimMode, ignoreCase, ignoreUmlauts, unifyFloatingNumbers, whitespacesMode)));

            var differences = matchSubstring ? DiffAsSubstring(submissionText, solutionText) : DiffViaDiffPlex(submissionText, solutionText);

            if (differences.Success == false && !string.IsNullOrWhiteSpace(differences.Details))
            {
                differences.Details += Environment.NewLine + "<p><b>Hinweis:</b> Der Text wurde in HTML convertiert und ggf. normalisiert. ";
                if (trimMode != TrimMode.None)
                {
                    differences.Details += "Die Zeilen wurden am Anfang/Ende getrimmt. ";
                }

                if (ignoreCase)
                {
                    differences.Details += "Es wurde alles in Kleinbuchstaben konvertiert. ";
                }

                if (ignoreUmlauts)
                {
                    differences.Details += "Alle Umlaute wurden ersetzt. ";
                }

                if (unifyFloatingNumbers)
                {
                    differences.Details += "Kommazahlen wurden vereinheitlicht. ";
                }

                if (whitespacesMode != WhitespacesMode.LeaveAsIs)
                {
                    differences.Details += "Leerzeichen wurden entfernt. ";
                }

                differences.Details += "</p>";
            }

            return (differences.Success, differences.Details, true);
        }

        private (bool Success, string Details) DiffAsSubstring(string actual, string expected)
        {
            return (actual.IndexOf(expected, StringComparison.CurrentCulture) >= 0, string.Empty);
        }

        private (bool Success, string Details) DiffViaDiffPlex(string actual, string expected)
        {
            var sb = new StringBuilder();
            var diffBuilder = new InlineDiffBuilder(new Differ());
            var diff = diffBuilder.BuildDiffModel(actual, expected);

            if (diff.Lines.All(x => x.Type == ChangeType.Unchanged))
            {
                return (true, string.Empty);
            }

            if (diff.Lines.All(x => x.Type != ChangeType.Unchanged))
            {
                return (false, string.Empty);
            }

            sb.Append("<pre><code>");
            foreach (var line in diff.Lines)
            {
                var color = false;
                switch (line.Type)
                {
                    case ChangeType.Inserted:
                        sb.Append(@"<span style=""color:green;"">+ ");
                        color = true;
                        break;
                    case ChangeType.Deleted:
                        sb.Append(@"<span style=""color:red;"">- ");
                        color = true;
                        break;
                    default:
                        sb.Append("  ");
                        break;
                }

                sb.Append(DiffHelper.HtmlEncodeDiffText(line.Text));
                if (color)
                {
                    sb.Append("</span>");
                }

                sb.AppendLine();
            }

            sb.AppendLine("</code></pre>");
            return (false, sb.ToString());
        }
    }
}
