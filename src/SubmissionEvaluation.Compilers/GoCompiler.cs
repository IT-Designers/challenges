using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class GoCompiler : CompilerBase
    {
        private readonly string compilerPath;
        private readonly ILog log;

        public GoCompiler(ILog log, string compilerPath)
        {
            this.log = log;
            this.compilerPath = compilerPath;
        }

        public override string Name => "Go";
        public override string Description => "-";
        public override string LatexCodeExtension => ".go";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(compilerPath, new[] {"version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".go", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var execName = "submission.exe";
            var pathToBinary = $"{paths.Sandbox.BinaryPath}/{execName}";
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.go", SearchOption.AllDirectories);
            log.Information("{count} go Dateien zum kompelieren gefunden.", compilableFiles.Length);

            var binaryDate = File.GetLastWriteTime(Path.Combine(paths.Host.BinaryPath, execName));
            if (!forceRecompile && compilableFiles.All(x => File.GetLastWriteTime(x) < binaryDate))
            {
                log.Information("Überspringe Kompilierung, da schon ein aktuelles Binary vorliegt");
            }
            else
            {
                CleanOutputBin(paths);
                ISyncLock compilelock = null;
                try
                {
                    compilelock = processProvider.GetLock(new[]
                    {
                        new FolderMapping {Source = paths.Host.SourcePath, Target = paths.Sandbox.SourcePath, ReadOnly = true},
                        new FolderMapping {Source = paths.Host.BinaryPath, Target = paths.Sandbox.BinaryPath}
                    });

                    var files = compilableFiles.Select(x =>
                        CompilerHelper.ConvertToSandboxPath(Path.Combine(paths.Sandbox.SourcePath, CompilerHelper.GetRelativePath(x, paths.Host.SourcePath))));
                    var result = processProvider.Execute(compilerPath, new[] {"build", "-o", pathToBinary}.Concat(files).ToArray(),
                        workingDir: paths.Sandbox.SourcePath, syncLock: compilelock).Result;
                    if (result.Timeout)
                    {
                        throw new CompilerException("Timout während Kompilierevorgang", result, Name);
                    }

                    if (result.ExitCode != 0)
                    {
                        throw new CompilerException("Kompiliervorgang fehlgeschlagen", result, Name);
                    }
                }
                finally
                {
                    processProvider.ReleaseLock(compilelock);
                }
            }

            return new ExecutionParameters {Path = pathToBinary, Arguments = new string[0], Language = Name, OutputEncoding = "UTF8"};
        }
    }
}
