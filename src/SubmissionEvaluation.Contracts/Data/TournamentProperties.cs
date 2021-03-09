using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data
{
    public class TournamentProperties : ITournament, IWithDescription
    {
        [YamlIgnore] [JsonIgnore] public bool IsActive => !HasError && !IsDraft && DateTime.Today >= Startdate && DateTime.Today <= Enddate;

        [YamlIgnore] public string Name { get; set; }

        [YamlMember(Alias = "Author")] public string AuthorID { get; set; }

        [YamlMember(Alias = "LastEditor")] public string LastEditorID { get; set; }

        public string Title { get; set; }
        public bool HasError { get; set; }
        public bool IsDraft { get; set; }
        public DateTime Date { get; set; }
        public DateTime Startdate { get; set; }
        public DateTime Enddate { get; set; }
        public List<string> Languages { get; set; }

        [YamlIgnore] [JsonIgnore] public string Description { get; set; }

        public string Game { get; set; }

        public string ErrorDescription { get; set; }
        public List<string> LimitedFor { get; set; }
    }
}
