using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal static class DiffHelper
    {
        private static readonly Dictionary<string, string> ansiColorLookup = new Dictionary<string, string>
        {
            {"\x1b[30m", "grey"},
            {"\x1b[30;1m", "grey"},
            {"\x1b[31m", "red"},
            {"\x1b[31;1m", "red"},
            {"\x1b[32m", "green"},
            {"\x1b[32;1m", "green"},
            {"\x1b[33m", "GoldenRod"},
            {"\x1b[33;1m", "GoldenRod"},
            {"\x1b[34m", "blue"},
            {"\x1b[34;1m", "blue"},
            {"\x1b[35m", "magenta"},
            {"\x1b[35;1m", "magenta"},
            {"\x1b[36m", "cyan"},
            {"\x1b[36;1m", "cyan"},
            {"\x1b[37m", "black"},
            {"\x1b[37;1m", "black"}
        };

        public static string HtmlEncodeDiffText(string lineText)
        {
            var text = HttpUtility.HtmlEncode(lineText).Replace(" ", "&middot;") + "&para;";

            if (text.Contains("\x1b["))
            {
                text = ReplaceAnsiEscapeCodeWithHtml(text);
            }

            return text.Replace("\x1b", "&larr;");
        }

        private static string ReplaceAnsiEscapeCodeWithHtml(string text)
        {
            var result = new StringBuilder();
            var matchSequence = "";
            var currentColor = "";
            foreach (var chr in text)
            {
                if (chr == '\x1b')
                {
                    matchSequence = "\x1b";
                }
                else
                {
                    if (matchSequence.Length == 0)
                    {
                        result.Append(chr);
                    }
                    else
                    {
                        matchSequence += chr.ToString().ToLower();
                    }
                }

                if (matchSequence == "\x1b[h")
                {
                    if (result.Length > 0)
                    {
                        result.AppendLine();
                    }

                    result.AppendLine("&mdash;");
                    matchSequence = "";
                }
                else if (matchSequence == "\x1b[2j")
                {
                    matchSequence = "";
                }
                else if (matchSequence == "\x1b[0m")
                {
                    if (currentColor != "")
                    {
                        result.Append("</span>");
                        currentColor = "";
                    }

                    matchSequence = "";
                }
                else if (matchSequence.Length > 0 && ansiColorLookup.TryGetValue(matchSequence, out var color))
                {
                    if (currentColor != color)
                    {
                        if (currentColor != "")
                        {
                            result.Append("</span>");
                        }

                        result.Append($"<span style=\"color:{color};\">");
                        currentColor = color;
                    }

                    matchSequence = "";
                }
            }

            if (currentColor != "")
            {
                result.Append("</span>");
            }

            result.Append(matchSequence);
            var s = result.ToString();
            return s;
        }

        public static string CleanString(string source, TrimMode trimMode, bool toLowerCase, bool replaceUmlauts, bool unifyFloatingNumbers,
            WhitespacesMode whitespacesMode)
        {
            if (unifyFloatingNumbers)
            {
                source = Regex.Replace(source, @"(\d+|\s+)(\,)(\d+)", "$1.$3");
            }

            var map = new Dictionary<char, string>();
            if (replaceUmlauts)
            {
                map.Add('ä', "ae");
                map.Add('ö', "oe");
                map.Add('ü', "ue");
                map.Add('Ä', "Ae");
                map.Add('Ö', "Oe");
                map.Add('Ü', "Ue");
                map.Add('ß', "ss");
            }

            if (whitespacesMode == WhitespacesMode.Reduce)
            {
                map.Add('\t', " ");
            }

            if (whitespacesMode == WhitespacesMode.Remove)
            {
                map.Add('\t', "");
                map.Add(' ', "");
            }

            if (whitespacesMode == WhitespacesMode.RemoveEvenNewlines)
            {
                map.Add('\t', "");
                map.Add(' ', "");
                map.Add('\n', "");
                map.Add('\r', "");
            }

            var lastCharWhitespace = false;
            var result = new StringBuilder(source.Length);
            foreach (var chr in source)
            {
                if (!map.TryGetValue(chr, out var r))
                {
                    r = chr.ToString();
                }

                if (r.Length <= 0 || whitespacesMode == WhitespacesMode.Reduce && lastCharWhitespace && r[0] == ' ')
                {
                    continue;
                }

                result.Append(toLowerCase ? r.ToLower() : r);
                lastCharWhitespace = r[0] == ' ';
            }

            return Trim(result.ToString(), trimMode);
        }

        private static string Trim(string line, TrimMode trimMode)
        {
            return trimMode switch
            {
                TrimMode.Start => line.TrimStart(),
                TrimMode.End => line.TrimEnd(),
                TrimMode.StartEnd => line.Trim(),
                _ => line
            };
        }
    }
}
