using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class TuCompiler : CompilerBase
    {
        private readonly ILog log;
        private readonly string runtimePath;

        public TuCompiler(ILog log, string runtimePath)
        {
            this.log = log;
            this.runtimePath = runtimePath;
        }

        public override string Name => "Tu";

        public override string Description =>
            "-";

        public override string LatexCodeExtension => ".tu";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(runtimePath, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".tu", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.tu", SearchOption.AllDirectories);
            log.Information("{count} Tu Dateien gefunden.", compilableFiles.Length);
            CleanOutputBin(paths);
            foreach (var file in compilableFiles)
            {
                var dest = Path.Combine(paths.Host.BinaryPath, CompilerHelper.GetRelativePath(file, paths.Host.SourcePath));
                var path = Path.GetDirectoryName(dest);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.Copy(file, dest, true);
            }

            string mainFile;
            if (compilableFiles.Length == 1)
            {
                mainFile = compilableFiles[0];
            }
            else
            {
                throw new CompilerException(
                    "Mehrere Tu-Dateien gefunden, aber es darf nur eine Tu-Datei eingereicht werden.", Name);
            }

            var mainFileRel = CompilerHelper.GetRelativePath(mainFile, paths.Host.SourcePath);
            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {mainFileRel},
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }
    }
}
