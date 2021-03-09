using System;

namespace SubmissionEvaluation.Providers.FileProvider
{
    public class CacheEntry
    {
        public string Path { get; set; }
        public object Content { get; set; }
        public DateTime FileDateTime { get; set; }
        public int AccessCounter { get; set; }
    }
}
