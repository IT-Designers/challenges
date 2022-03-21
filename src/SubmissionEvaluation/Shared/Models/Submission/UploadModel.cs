using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Models.Shared;

namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class UploadModel<T, TS> : SubmissionHistoryModel<TS> where T : ISubmission where TS : IMember
    {
        public UploadModel()
        {
            UploadArchive = UploadArchive ?? new List<DetailedInputFile>();
        }

        public string ChallengeTitle { get; set; }
        public List<DetailedInputFile> UploadArchive { get; set; }
        public string SubmissionId { get; set; }
        public T SelectedSubmission { get; set; }
    }
}
