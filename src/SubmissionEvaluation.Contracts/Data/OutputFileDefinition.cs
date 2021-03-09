using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class OutputFileDefinition
    {
        public string Name { get; set; }

        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Content { get; set; }

        public CompareSettings Settings { get; set; } = new CompareSettings();
    }
}
