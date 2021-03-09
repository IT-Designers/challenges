using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class TypeScriptCompiler : CompilerBase
    {
        private readonly string compilerPath;
        private readonly ILog log;

        private readonly string npmPath;

        private readonly string runtimePath;

        public TypeScriptCompiler(ILog log, string compilerPath, string runtimePath, string npmPath)
        {
            this.log = log;
            this.compilerPath = compilerPath;
            this.runtimePath = runtimePath;
            this.npmPath = npmPath;
        }

        public override string LatexCodeExtension => ".ts";

        public override string Name => "TypeScript";

        public override string Description =>
            @"Zum Ausführen wird NodeJS verwendet.

Wenn keine NodeJS Konfiguration vorliegt, wird die erste gefundene Datei als Startpunkt festgelegt.";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(compilerPath, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return "TypeScript " + result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".ts", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.ts", SearchOption.AllDirectories);
            log.Information("{count} Typescript Dateien gefunden.", compilableFiles.Length);

            CleanOutputBin(paths);
            ISyncLock compilelock = null;
            try
            {
                compilelock = processProvider.GetLock(new[]
                {
                    new FolderMapping {Source = paths.Host.SourcePath, Target = paths.Sandbox.SourcePath, ReadOnly = true},
                    new FolderMapping {Source = paths.Host.BinaryPath, Target = paths.Sandbox.BinaryPath}
                });

                CompileTsFiles(compilableFiles, paths, processProvider, compilelock);
                CopyConfigFiles(paths);

                if (File.Exists(Path.Combine(paths.Host.SourcePath, "config.json")) || File.Exists(Path.Combine(paths.Host.SourcePath, "package.json")))
                {
                    log.Information("NPM Package Konfiguration gefunden.");
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
                        Path = runtimePath,
                        Arguments = new[] {"."},
                        WorkingDirectory = paths.Sandbox.BinaryPath,
                        OutputEncoding = "UTF8",
                        Language = Name
                    };
                }

                log.Information("Keine NPM Package Konfiguration gefunden. Nehme erste .js-Datei und nodejs als Executable");
                var compiledFiles = Directory.GetFiles(paths.Host.BinaryPath, "*.js", SearchOption.AllDirectories);
                var mainFile = CompilerHelper.GetRelativePath(compiledFiles.First(), paths.Host.BinaryPath);

                return new ExecutionParameters {Path = runtimePath, Arguments = new[] {mainFile}, WorkingDirectory = paths.Sandbox.BinaryPath, Language = Name};
            }
            finally
            {
                processProvider.ReleaseLock(compilelock);
            }
        }

        #region Helper Methods

        private void CompileTsFiles(string[] compilableFiles, CompilerPaths paths, IProcessProvider processProvider, ISyncLock compilelock)
        {
            var files = compilableFiles.Select(x =>
                CompilerHelper.ConvertToSandboxPath(Path.Combine(paths.Sandbox.SourcePath, CompilerHelper.GetRelativePath(x, paths.Host.SourcePath))));
            var compilerOptions = BuildCompilerOptions(files, paths.Sandbox.SourcePath, paths.Sandbox.BinaryPath);
            var result = processProvider.Execute(compilerPath, compilerOptions, workingDir: paths.Sandbox.BinaryPath, syncLock: compilelock).Result;
            if (result.Timeout)
            {
                throw new CompilerException("Timout während Kompilierung", result, Name);
            }

            if (result.ExitCode != 0)
            {
                throw new CompilerException("Kompilierung fehlgeschlagen", result, Name);
            }
        }

        private string[] BuildCompilerOptions(IEnumerable<string> compilableFiles, string pathToContent, string buildPath)
        {
            if (File.Exists(Path.Combine(pathToContent, "tsconfig.json")))
            {
                log.Information("tsconfig.json Datei gefunden. Kompiliere nach dieser.");
                return new[] {"--outDir", buildPath, "--sourceMap", "false"};
            }

            log.Information("Keine tsconfig.json Datei gefunden. Kompiliere alle .ts Dateien.");
            return compilableFiles.Concat(new[] {"--outDir", buildPath}).ToArray();
        }

        private void CopyConfigFiles(CompilerPaths paths)
        {
            log.Information("Kopiere alle nicht kompilierbaren Dateien in das Build Verzeichnis.");
            var configFiles = GetNonTsFiles(paths.Host.SourcePath).Where(IsConfigFile).ToList();
            configFiles.ForEach(f => CompilerHelper.Copy(f, Path.Combine(paths.Host.BinaryPath, CompilerHelper.GetRelativePath(f, paths.Host.SourcePath))));
        }

        private bool IsConfigFile(string file)
        {
            var fileName = Path.GetFileName(file);
            return fileName == "config.json" || fileName == "package.json";
        }

        private static List<string> GetNonTsFiles(string pathToContent)
        {
            return GetAllFiles(pathToContent).Where(f => Path.GetExtension(f) != ".ts").ToList();
        }

        private static string[] GetAllFiles(string pathToContent)
        {
            return Directory.GetFiles(pathToContent, "*.*", SearchOption.AllDirectories);
        }

        #endregion
    }
}
