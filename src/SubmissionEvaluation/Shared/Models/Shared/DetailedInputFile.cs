namespace SubmissionEvaluation.Shared.Models.Shared
{
    public class DetailedInputFile : File
    {
        public DetailedInputFile(File file) : base(file)
        {
        }

        public DetailedInputFile()
        {
        }

        public byte[] Content { get; set; }
    }
}
