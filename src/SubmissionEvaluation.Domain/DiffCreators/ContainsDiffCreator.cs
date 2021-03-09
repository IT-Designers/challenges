using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal class ContainsDiffCreator : IDiffCreator
    {
        private readonly bool ignoreCase;
        private readonly bool ignoreUmlauts;
        private readonly bool mustBeWord;
        private readonly TrimMode trimMode;
        private readonly bool unifyFloatingNumbers;
        private readonly WhitespacesMode whitespacesMode;

        public ContainsDiffCreator(TrimMode trimMode, bool ignoreCase, bool ignoreUmlauts, bool unifyFloatingNumbers, WhitespacesMode whitespacesMode,
            bool mustBeWord)
        {
            this.trimMode = trimMode;
            this.ignoreCase = ignoreCase;
            this.ignoreUmlauts = ignoreUmlauts;
            this.unifyFloatingNumbers = unifyFloatingNumbers;
            this.whitespacesMode = whitespacesMode;
            this.mustBeWord = mustBeWord;
        }

        public (bool Success, string Details, bool showExpected) GetDiff(string actual, string expected)
        {
            var outputLines = whitespacesMode == WhitespacesMode.RemoveEvenNewlines ? new[] {actual} : Regex.Split(actual.Trim(), "\r\n|\n");
            var expectedLines = Regex.Split(expected.Trim(), "\r\n|\n");
            var expectedLineIndex = 0;
            var currentSeekText = DiffHelper.CleanString(expectedLines[0], trimMode, ignoreCase, ignoreUmlauts, unifyFloatingNumbers, whitespacesMode);

            var matchResult = new StringBuilder();
            matchResult.AppendLine("Es folgt die Programmausgabe. Treffer wurden jeweils am Anfang der Zeile gekennzeichnet:");
            matchResult.Append("<pre><code>");

            var maxDigits = expectedLines.Length.ToString().Length;
            var lastMatch = -1;
            var lines = new List<string>();
            foreach (var line in outputLines)
            {
                var currentLine = DiffHelper.CleanString(line, trimMode, ignoreCase, ignoreUmlauts, unifyFloatingNumbers, whitespacesMode);
                if (currentLine.IndexOf(currentSeekText, StringComparison.CurrentCulture) >= 0)
                {
                    // Filter all matches on same line
                    int index;
                    while ((index = currentLine.IndexOf(currentSeekText, StringComparison.CurrentCulture)) >= 0)
                    {
                        var lastChar = index > 0 ? currentLine[index - 1] : ' ';
                        var nextChar = index + currentSeekText.Length < currentLine.Length - 1 ? currentLine[index + currentSeekText.Length] : ' ';
                        if (!mustBeWord || IsWordSeperator(lastChar) && IsWordSeperator(nextChar))
                        {
                            lines.Add($"<b>{(expectedLineIndex + 1).ToString().PadLeft(maxDigits)}>{DiffHelper.HtmlEncodeDiffText(line)}</b>");
                            lastMatch = lines.Count;

                            expectedLineIndex++;
                            if (expectedLineIndex >= expectedLines.Length)
                            {
                                return (true, string.Empty, false);
                            }

                            var pos = currentLine.IndexOf(currentSeekText, StringComparison.CurrentCulture) + currentSeekText.Length;
                            currentLine = pos < currentLine.Length ? currentLine[pos..] : "";
                            currentSeekText = DiffHelper.CleanString(expectedLines[expectedLineIndex], trimMode, ignoreCase, ignoreUmlauts,
                                unifyFloatingNumbers, whitespacesMode);
                        }
                        else
                        {
                            currentLine = currentLine[(index + 1)..];
                        }
                    }
                }
                else
                {
                    lines.Add(new string(' ', maxDigits + 1) + DiffHelper.HtmlEncodeDiffText(line));
                }
            }

            for (var i = 0; i < lines.Count; i++)
            {
                if (i < lastMatch)
                {
                    matchResult.AppendLine(@"<span style=""color:green;"">" + lines[i] + "</span>");
                }
                else
                {
                    matchResult.AppendLine(@"<span style=""color:red;"">" + lines[i] + "</span>");
                }
            }

            matchResult.AppendLine("</code></pre>");

            var result = new StringBuilder();
            result.AppendLine("<p>Es wurde nach folgenden Textteilen gesucht (Suchreihenfolge erfolgte von oben nach unten):</p>");
            result.Append("<pre><code>");
            for (var i = 0; i < expectedLineIndex; i++)
            {
                result.AppendLine($@"<span style=""color:green;"">&#x2714; {i + 1}. {HttpUtility.HtmlEncode(expectedLines[i])}</span>");
            }

            for (var i = expectedLineIndex; i < expectedLines.Length; i++)
            {
                result.AppendLine($@"<span style=""color:red;"">&#x2718; {i + 1}. {HttpUtility.HtmlEncode(expectedLines[i])}</span>");
            }

            result.AppendLine();
            result.AppendLine("</code></pre>");

            if (expectedLineIndex < expectedLines.Length)
            {
                if (expectedLineIndex == 0)
                {
                    result.Insert(0,
                        $"<p>Es wurde folgender Text in der vom Programm gelieferten Ausgabe nicht gefunden: </p><pre><code>{HttpUtility.HtmlEncode(expectedLines[expectedLineIndex])}</code></pre><p><b>Hinweis:</b> Bitte auf genaue Schreibweise achten.</p>");
                }
                else
                {
                    result.Insert(0,
                        $"<p>Es wurde folgender Text in der vom Programm gelieferten Ausgabe nicht gefunden: </p><pre><code>{HttpUtility.HtmlEncode(expectedLines[expectedLineIndex])}</code></pre><p><b>Hinweis:</b> Bitte auf genaue Schreibweise achten. Dieser muss nach folgender Ausgabe erfolgen:</p><pre><code>{HttpUtility.HtmlEncode(expectedLines[expectedLineIndex - 1])}</code></pre>");
                }
            }

            result.AppendLine(matchResult.ToString());

            if (expectedLines.Length <= 1)
            {
                result.Clear();
            }

            return (false, result.ToString(), expectedLines.Length <= 1);
        }

        private bool IsWordSeperator(char chr)
        {
            if (char.IsLetterOrDigit(chr))
            {
                return false;
            }

            if (chr == '-')
            {
                return false;
            }

            return true;
        }
    }
}
