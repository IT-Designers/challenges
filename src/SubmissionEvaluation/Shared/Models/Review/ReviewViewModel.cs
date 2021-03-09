using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Shared.Models.Review
{
    public class ReviewViewModel : GenericModel
    {
        public string ChallengeId { get; set; }
        public string SubmissionId { get; set; }
        public ReviewData ReviewData { get; set; }
        public dynamic[] FileModel { get; set; }
        public int Stars { get; set; }
        public IEnumerable<ReviewFile> SourceFiles { get; set; }
        public ReviewRating ReviewRating { get; set; }
        public List<(string Id, string Title)> Categories { get; set; }
        public Dictionary<string, CommentModel[]> Comments { get; set; }

        public int CalculateReviewRating(ReviewRating rating) //TODO: COPY FROM REVIEW OPERATIONS REMOVE
        {
            if (rating.Rating == null)
            {
                return 0;
            }

            var ratingValue = rating.Rating.Value;
            if (ratingValue <= 1.5)
            {
                return 5;
            }

            if (ratingValue <= 2)
            {
                return 4;
            }

            if (ratingValue <= 2.7)
            {
                return 3;
            }

            if (ratingValue <= 4)
            {
                return 2;
            }

            return 1;
        }
    }
}
