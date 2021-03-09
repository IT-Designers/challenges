namespace SubmissionEvaluation.Shared.Models.Test
{
    public class OutputFile
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public CompareSettingsModel CompareSettings { get; set; } = new CompareSettingsModel();
    }
}
