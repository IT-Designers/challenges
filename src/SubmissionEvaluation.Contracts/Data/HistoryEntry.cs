using System;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class HistoryEntry
    {
        public string Challenge { get; set; }
        public HistoryType Type { get; set; }
        public DateTime Date { get; set; }
        public EvaluationResult? Result { get; set; }
        public string Id { get; set; }
        public int? Stars { get; set; }
        public string Language { get; set; }

        [YamlIgnore] [JsonIgnore] public bool IsPassed => Result == EvaluationResult.Succeeded || Result == EvaluationResult.SucceededWithTimeout;
    }
}
