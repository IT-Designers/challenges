using System.Collections.Generic;
using SubmissionEvaluation.Shared.Models.Shared;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class UploadChallengeModel : GenericModel
    {
        public List<DetailedInputFile> UploadedFiles { get; set; } = new List<DetailedInputFile>();
    }
}
