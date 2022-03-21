using YamlDotNet.Serialization;

namespace SubmissionEvaluation.Contracts.Data.Review
{

    public class ReviewLevelAndCounter
    {
        public ReviewLevelType ReviewLevel{get; set;}
        public uint ReviewCounter {get; set;}
        [YamlIgnore]
        public bool Selected {get; set;}
    }
    public enum ReviewLevelType : uint
    {
        Inactive,
        Beginner,
        Intermediate,
        Advanced,
        Expert,
        Master,
        Deactivated
    }
}
