using System;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentEntry
    {
        public string Id { get; set; }

        [YamlIgnore] [JsonIgnore] public string SubmissionPath { get; set; }

        public DateTime FirstSubmission { get; set; }
        public int Elo { get; set; }
        public int Win { get; set; }
        public int Draw { get; set; }
        public int Loss { get; set; }
        public int RankPoints { get; set; }
        public int Matches { get; set; }
        public int Updates { get; set; }
        public TournamentEntryState State { get; set; }
        public DateTime Updated { get; set; }
        public string Team { get; set; }
        public long ProcessingTurns { get; set; }
        public long ProcessingTime { get; set; }
        public long MaxProcessingTime { get; set; }
        public string WorkinDirectory { get; set; }
        public string StartCommand { get; set; }
        public string[] StartArguments { get; set; }
        public string Language { get; set; }

        [YamlIgnore] [JsonIgnore] public string Tournament { get; set; }
    }
}
