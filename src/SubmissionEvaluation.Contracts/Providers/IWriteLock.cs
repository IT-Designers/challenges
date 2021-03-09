using System;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface IWriteLock : IDisposable
    {
        void EnsureWriteLock(string path);
    }
}
