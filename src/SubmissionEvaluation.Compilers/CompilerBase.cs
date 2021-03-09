using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public abstract class CompilerBase : ICompiler
    {
        public string VersionDetails { get; set; } = "";
        public string Version { get; set; }
        public string[] CompilerSwitches { get; set; } = new string[0];
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract string LatexCodeExtension { get; }
        public bool Available { get; set; }

        public bool CanCompileContent(string pathToContent)
        {
            return CanCompileContent(Directory.EnumerateFiles(pathToContent, "*", SearchOption.AllDirectories));
        }

        public virtual bool CanCompileContent(IEnumerable<string> files)
        {
            return GetSourceFiles(files).Any();
        }

        public virtual IEnumerable<string> GetSourceFiles(string pathToContent)
        {
            return GetSourceFiles(Directory.EnumerateFiles(pathToContent, "*", SearchOption.AllDirectories));
        }

        public abstract ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile);

        public abstract string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock);
        public abstract IEnumerable<string> GetSourceFiles(IEnumerable<string> files);

        public SizeInfo DetermineContentSize(string pathToContent)
        {
            var di = GetSourceFiles(pathToContent);
            var filesize = di.Select(x => new FileInfo(x).Length).Sum();
            return new SizeInfo {SizeInBytes = filesize};
        }

        public void CleanOutputBin(CompilerPaths paths)
        {
            var dirInfo = new DirectoryInfo(paths.Host.BinaryPath);
            if (dirInfo.Exists)
            {
                foreach (var dir in dirInfo.EnumerateDirectories())
                {
                    dir.Delete(true);
                }

                foreach (var file in dirInfo.EnumerateFiles())
                {
                    file.Delete();
                }
            }
        }
    }
}
