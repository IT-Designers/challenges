using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class Bundle : IBundle, IWithDescription
    {
        public string Title { get; set; }

        [YamlIgnore] [JsonIgnore] public string Id { get; set; }

        [YamlIgnore] [JsonIgnore] public string Description { get; set; }

        public string Author { get; set; }
        public bool IsDraft { get; set; }
        public List<string> Challenges { get; set; } = new List<string>();
        public string Category { get; set; }
        public bool HasPreviousChallengesCheck { get; set; }

        public BundleState State { get; set; } = new BundleState();
        public string LearningFocus { get; set; }

        public override string ToString()
        {
            return $"Bundle: {Id}";
        }
    }
}
