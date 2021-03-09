using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Test
{
    public class CompareSettingsModel
    {
        public CompareModeType CompareMode { get; set; } = CompareModeType.Exact;
        public bool IsUnifyFloatingNumbers { get; set; } = false;
        public bool IsIncludeCase { get; set; } = false;
        public bool IsKeepUmlauts { get; set; } = false;
        public TrimMode Trim { get; set; } = TrimMode.StartEnd;
        public WhitespacesMode Whitespaces { get; set; } = WhitespacesMode.LeaveAsIs;
        public double Threshold { get; set; } = 0;
    }
}
