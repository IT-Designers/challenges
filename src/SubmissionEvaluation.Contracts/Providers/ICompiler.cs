using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Contracts.Providers
{
    public interface ICompiler
    {
        string LatexCodeExtension { get; }
        string Name { get; }
        string Version { get; set; }
        string VersionDetails { get; set; }
        bool Available { get; set; }
        string Description { get; }
        string[] CompilerSwitches { get; }
        bool CanCompileContent(IEnumerable<string> files);
        bool CanCompileContent(string pathToContent);
        ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile);
        SizeInfo DetermineContentSize(string pathToContent);
        IEnumerable<string> GetSourceFiles(IEnumerable<string> files);
        IEnumerable<string> GetSourceFiles(string pathToContent);
        string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock);
    }
}
