using System.Text.RegularExpressions;

namespace SubmissionEvaluation.Server.Classes.JSON
{
    public class Base64Extractor
    {
        private static readonly string patternJsonBase64 = "base64,(.*)";

        public static string FromJsonBase64(string jsonBase64String)
        {
            var regexer = new Regex(patternJsonBase64);
            var m = regexer.Match(jsonBase64String);
            return m.Groups[1].Value;
        }
    }
}
