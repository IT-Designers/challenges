using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class JuliaCompiler : CompilerBase
    {
        private readonly ILog log;
        private readonly string runtimePath;

        public JuliaCompiler(ILog log, string runtimePath)
        {
            this.log = log;
            this.runtimePath = runtimePath;
        }

        public override string Name => "Julia";

        public override string Description => "Unterstützt Einreichungen nur in einer Datei, da ansonsten Einstiegspunkt für Programmstart unklar.";

        public override string LatexCodeExtension => ".jl";

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
            return files.Where(x => x.EndsWith(".jl", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.jl", SearchOption.AllDirectories);
            log.Information("{count} Julia Dateien gefunden.", compilableFiles.Length);

            CleanOutputBin(paths);
            foreach (var file in compilableFiles)
            {
                File.Copy(file, Path.Combine(paths.Host.BinaryPath, CompilerHelper.GetRelativePath(file, paths.Host.SourcePath)), true);
            }

            // TODO: Will only use first file
            if (compilableFiles.Length > 1)
            {
                throw new CompilerException("Mehrere Julia-Dateien gefunden. Momentan nicht unterstützt, da Einstiegspunkt für Programmstart unklar.", Name);
            }

            var mainFile = CompilerHelper.GetRelativePath(compilableFiles.First(), paths.Host.SourcePath);

            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {mainFile},
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }
    }
}
