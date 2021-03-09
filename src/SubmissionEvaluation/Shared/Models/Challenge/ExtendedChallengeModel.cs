using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Models.Shared;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class ExtendedChallengeModel : ChallengeModel
    {
        public ExtendedChallengeModel()
        {
        }

        public ExtendedChallengeModel(IChallenge c) : base(c)
        {
            NewFiles = new List<DetailedInputFile>();
        }

        public List<DetailedInputFile> NewFiles { get; set; }
    }
}
