using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Models.Challenge;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class TestGeneratorModel<T, S> : ChallengeTestCreateModel where T : ISubmission where S : IMember
    {
        public delegate void submissionChanged();

        private string _submissionId;
        public IReadOnlyList<SubmissionModel<T, S>> AvailableSubmissions { get; set; }
        public string ChallengeName { get; set; }

        public string SubmissionId
        {
            get => _submissionId;
            set
            {
                _submissionId = value;
                SubmissionChanged?.Invoke();
            }
        }

        public event submissionChanged SubmissionChanged;
    }
}
