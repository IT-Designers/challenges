using System.Collections.Generic;
using System.Threading.Tasks;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface IProcessProvider
    {
        Task<ProcessResult> Execute(string path, string[] arguments, string input = null, string workingDir = null, int timeout = 60000, string encoding = null,
            List<FileDefinition> inputFiles = null, IDictionary<string, string> env = null, FolderMapping[] folderMappings = null, ISyncLock syncLock = null);

        ISyncLock GetLock(FolderMapping[] folders = null, InteresstedFileChanges[] changes = null);
        void ReleaseLock(ISyncLock lockObject);
    }
}
