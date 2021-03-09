using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Shared.Models.Review;

namespace SubmissionEvaluation.Client.Shared.Models
{
    public class FileReviewCommentsAssoziater
    {
        public FileReviewCommentsAssoziater(ReviewCodeComments comments, ReviewFile sourceFile)
        {
            Comments = comments;
            SourceFile = sourceFile;
        }

        public ReviewCodeComments Comments { get; internal set; }
        public ReviewFile SourceFile { get; internal set; }
    }
}
