using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Shared.Models.Shared;

namespace SubmissionEvaluation.Client
{
    public static class InputButtonHelper
    {
        public const string GetNormalColor = "color:black;";

        private static bool SpecialCharacterInvolved(string name)
        {
            return name.Contains(Folder.PathSeperator) && name.Contains("|") && name.Contains("/") && name.Contains("\\") && name.Contains("*");
        }

        public static bool CheckValidity(IEnumerable<File> files, IEnumerable<DetailedInputFile> otherFiles, string name, File file)
        {
            return !files.Any(x => x.Name.Equals(name) && !x.Equals(file)) && !otherFiles.Any(x => x.Name.Equals(name) && !x.Equals(file)) &&
                   !SpecialCharacterInvolved(name);
        }
    }
}
