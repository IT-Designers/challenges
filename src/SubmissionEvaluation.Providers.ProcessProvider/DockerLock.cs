using System;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Providers.ProcessProvider
{
    public class DockerLock : ISyncLock
    {
        private readonly DockerProcessProvider provider;

        public DockerLock(DockerProcessProvider provider)
        {
            this.provider = provider;
        }

        public string DockerId { get; set; }
        public FolderMapping[] Folders { get; set; }
        public InteresstedFileChanges[] Changes { get; set; }

        public bool LockActive { get; set; }

        public byte[] GetFile(string filename)
        {
            return provider.GetFile(this, filename).Data;
        }

        public void WriteAllText(string path, string content)
        {
            provider.CopyTextToDockerImage2(this, content, path);
        }

        public bool IsDirty { get; set; }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        private void ReleaseUnmanagedResources()
        {
            provider?.ReleaseLock(this);
        }

        ~DockerLock()
        {
            ReleaseUnmanagedResources();
        }
    }
}
