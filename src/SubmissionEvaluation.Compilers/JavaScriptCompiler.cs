using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class JavaScriptCompiler : CompilerBase
    {
        private readonly ILog log;
        private readonly string npmPath;
        private readonly string runtimePath;

        public JavaScriptCompiler(ILog log, string runtimePath, string npmPath)
        {
            this.log = log;
            this.runtimePath = runtimePath;
            this.npmPath = npmPath;
        }

        public override string Name => "JavaScript";
        public override string Description => "Zum Ausführen wird NodeJS verwendet.";
        public override string LatexCodeExtension => ".js";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(runtimePath, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return "NodeJS " + result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".js", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.js", SearchOption.AllDirectories);
            var copyFiles = Directory.GetFiles(paths.Host.SourcePath, "*.*", SearchOption.AllDirectories);
            log.Information("{count} JavaScript Dateien gefunden.", compilableFiles.Length);

            CleanOutputBin(paths);
            foreach (var file in copyFiles)
            {
                CompilerHelper.Copy(file, Path.Combine(paths.Host.BinaryPath, CompilerHelper.GetRelativePath(file, paths.Host.SourcePath)));
            }

            if (File.Exists(Path.Combine(paths.Host.SourcePath, "config.json")) || File.Exists(Path.Combine(paths.Host.SourcePath, "package.json")))
            {
                log.Information("NPM Package Konfiguration gefunden.");
                ISyncLock compilelock = null;
                try
                {
                    compilelock = processProvider.GetLock(new[]
                    {
                        new FolderMapping {Source = paths.Host.SourcePath, Target = paths.Sandbox.SourcePath, ReadOnly = true},
                        new FolderMapping {Source = paths.Host.BinaryPath, Target = paths.Sandbox.BinaryPath}
                    });

                    var result = processProvider.Execute(npmPath, new[] {"install", "--production"},
                        workingDir: paths.Sandbox.SourcePath, syncLock: compilelock).Result;
                    if (result.Timeout)
                    {
                        throw new CompilerException("Timout während 'npm install'", result, Name);
                    }

                    if (result.ExitCode != 0)
                    {
                        throw new CompilerException("'npm install' fehlgeschlagen", result, Name);
                    }

                    return new ExecutionParameters
                    {
                        Path = npmPath,
                        Arguments = new[] {"start", "--silent"},
                        WorkingDirectory = paths.Sandbox.BinaryPath,
                        OutputEncoding = "UTF8",
                        Language = Name
                    };
                }
                finally
                {
                    processProvider.ReleaseLock(compilelock);
                }
            }

            log.Information("Keine NPM Package Konfiguration gefunden. Nehme erste .js-Datei und nodejs als Executable");
            var mainFile = CompilerHelper.GetRelativePath(compilableFiles.First(), paths.Host.SourcePath);

            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {mainFile}.ToArray(),
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }
    }
}
