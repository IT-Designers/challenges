using System;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class FileDefinition
    {
        public string Name { get; set; }

        public string ContentFile { get; set; }

        public DateTime? LastModified { get; set; }

        [YamlIgnore] [JsonIgnore] public string ContentFilePath { get; set; }
    }
}
