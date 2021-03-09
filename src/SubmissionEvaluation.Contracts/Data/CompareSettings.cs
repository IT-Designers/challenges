namespace SubmissionEvaluation.Contracts.Data
{
    public class CompareSettings
    {
        public TrimMode Trim { get; set; }
        public bool IncludeCase { get; set; }
        public bool KeepUmlauts { get; set; }
        public bool UnifyFloatingNumbers { get; set; }
        public WhitespacesMode Whitespaces { get; set; }
        public CompareModeType CompareMode { get; set; }
        public double Threshold { get; set; }
    }
}
