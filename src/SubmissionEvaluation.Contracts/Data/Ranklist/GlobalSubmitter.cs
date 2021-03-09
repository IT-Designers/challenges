namespace SubmissionEvaluation.Contracts.Data.Ranklist
{
    public class GlobalSubmitter
    {
        public int Rank { get; set; }
        public int LastPeriodRank { get; set; }
        public string Id { get; set; }
        public int SubmissionCount { get; set; }
        public int Points { get; set; }
        public int SolvedCount { get; set; }
        public int ChallengesCreated { get; set; }
        public int Stars { get; set; }
        public int RankChange { get; set; }
        public int ReceivedReviews { get; set; }
        public string Languages { get; set; }
        public int DuplicationScore { get; set; }
    }
}
