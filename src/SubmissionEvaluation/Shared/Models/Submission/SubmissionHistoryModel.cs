using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Submission
{
    public class SubmissionHistoryModel<T> : GenericModel where T : IMember
    {
        public SubmissionHistoryModel()
        {
            Submissions = Submissions ?? new List<SubmissionResult<T>>();
        }

        public string Id { get; set; }
        public List<SubmissionResult<T>> Submissions { get; set; }
        public string SubmissionDetails { get; set; }

        public bool AreSubmissionsSelectable { get; set; }

        //This replaces the SubmissionDetails which are a broken html version of below.
        public List<HintCategory> ErrorDetails { get; set; }
    }
}
