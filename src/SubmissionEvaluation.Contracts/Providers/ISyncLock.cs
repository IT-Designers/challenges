using System;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface ISyncLock : IDisposable
    {
        bool LockActive { get; }
        bool IsDirty { get; }
        byte[] GetFile(string filename);
        void WriteAllText(string path, string content);
    }
}
