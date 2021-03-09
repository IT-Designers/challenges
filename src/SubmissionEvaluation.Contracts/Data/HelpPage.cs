using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class HelpPage : IWithDescription
    {
        public string Title { get; set; }
        public string Parent { get; set; }

        [YamlIgnore] public string Path { get; set; }

        [YamlIgnore] public List<HelpPage> SubPages { get; set; } = new List<HelpPage>();

        [YamlIgnore] public string Description { get; set; }
    }
}
