namespace SubmissionEvaluation.Contracts.Data
{
    public class SubmitterRankingEntry
    {
        public string Challenge { get; set; }
        public int Rank { get; set; }
        public int Points { get; set; }
        public string Language { get; set; }
        public int DuplicateScore { get; set; }
    }
}
