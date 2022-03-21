using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Models.Challenge;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class TestGeneratorModel<T, TS> : ChallengeTestCreateModel where T : ISubmission where TS : IMember
    {
        public delegate void SubmissionChanged();

        private string submissionId;
        public IReadOnlyList<SubmissionModel<T, TS>> AvailableSubmissions { get; set; }
        public string ChallengeName { get; set; }

        public string SubmissionId
        {
            get => submissionId;
            set
            {
                submissionId = value;
                SubmissionChangedEvent?.Invoke();
            }
        }

        public event SubmissionChanged SubmissionChangedEvent;
    }
}
