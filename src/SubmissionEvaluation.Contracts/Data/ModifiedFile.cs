using System;

namespace SubmissionEvaluation.Contracts.Data
{
    public class ModifiedFile
    {
        public string Filename { get; set; }
        public string Realfilename { get; set; }
        public byte[] Data { get; set; }
        public DateTime? Date { get; set; }
    }
}
