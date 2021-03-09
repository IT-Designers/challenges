namespace SubmissionEvaluation.Contracts.Data
{
    public class RatingPoints
    {
        public RatingPoints(int last, int mid, int best)
        {
            Last = last;
            Mid = mid;
            Best = best;
        }

        public RatingPoints()
        {
        }

        public int Best { get; internal set; }
        public int Mid { get; internal set; }
        public int Last { get; internal set; }
    }
}
