using System.Collections.Generic;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class OutputDefinition
    {
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Content { get; set; }

        public List<string> Alternatives { get; set; }
        public CompareSettings Settings { get; set; } = new CompareSettings();
    }
}
