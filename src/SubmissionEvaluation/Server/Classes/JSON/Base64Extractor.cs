using System.Text.RegularExpressions;

namespace SubmissionEvaluation.Server.Classes.JSON
{
    public class Base64Extractor
    {
        private static readonly string patternJsonBase64 = "base64,(.*)";

        public static string FromJsonBase64(string jsonBase64string)
        {
            var regexer = new Regex(patternJsonBase64);
            var m = regexer.Match(jsonBase64string);
            return m.Groups[1].Value;
        }
    }
}
