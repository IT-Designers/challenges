using System;
using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.ClientPocos
{
    public class Game : IGame
    {
        public string AuthorID { get; set; }
        public string LastEditorID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool IsDraft { get; set; }
        public List<List<string>> Starterpacks { get; set; }
        public string StarterpacksIntroduction { get; set; }
        public DateTime Date { get; set; }
        public List<string> Languages { get; set; }
        public string Description { get; set; }
        public bool HasError { get; set; }
        public string ErrorDescription { get; set; }
        public List<string> Files { get; set; }
        public RunnerPropeties Verifier { get; set; }
        public RunnerPropeties MatchRunner { get; set; }
        public List<int> PossiblePlayersPerMatch { get; set; }
        public int MatchTimeout { get; set; }
    }
}
