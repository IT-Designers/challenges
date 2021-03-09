using System.IO;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Providers.ProcessProvider
{
    public class SyncLock : ISyncLock
    {
        public bool LockActive { get; set; }

        public byte[] GetFile(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        public void WriteAllText(string path, string content)
        {
            File.WriteAllText(path, content);
        }

        public bool IsDirty { get; }

        public void Dispose()
        {
        }
    }
}
