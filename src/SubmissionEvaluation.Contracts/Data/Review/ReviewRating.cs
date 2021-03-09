using System.Collections.Generic;
using System.Linq;

namespace SubmissionEvaluation.Contracts.Data.Review
{
    public class ReviewRating
    {
        private double? rating;
        public string Id { get; set; }
        public List<ReviewRating> Childs { get; } = new List<ReviewRating>();

        public double? Rating
        {
            get
            {
                var available = Childs.Where(x => x.Rating.HasValue).OrderByDescending(x => x.Rating).Take(3).ToList();
                if (available.Count > 0)
                {
                    return available.Sum(x => x.Rating * x.Quantifier) / available.Sum(x => x.Quantifier);
                }

                return rating;
            }
            set => rating = value;
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public int Quantifier { get; set; }
    }
}
