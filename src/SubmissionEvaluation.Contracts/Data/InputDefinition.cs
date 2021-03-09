using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class InputDefinition
    {
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Content { get; set; }
    }
}
