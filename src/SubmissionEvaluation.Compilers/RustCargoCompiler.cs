using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SubmissionEvaluation.Compilers.Helper;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class RustCargoCompiler : CompilerBase
    {
        private readonly string cargoPath;
        private readonly string compilerPath;
        private readonly ILog log;

        public RustCargoCompiler(ILog log, string cargoPath, string compilerPath)
        {
            this.log = log;
            this.cargoPath = cargoPath;
            this.compilerPath = compilerPath;
            CompilerSwitches = new[] {"-C", "opt-level=3", "-C", "debuginfo=0", "-D", "warnings"};
        }

        public override string Name => "Rust";
        public override string Description => "";
        public override string LatexCodeExtension => ".rs";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var version = ReadCargoVersionDetails(processProvider, versionlock);
            version += ReadRustVersionDetails(processProvider, versionlock);
            return version;
        }

        protected string ReadRustVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(compilerPath, new[] {"-V"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output.Split('(').First();
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        protected string ReadCargoVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(cargoPath, new[] {"-V"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output.Split('(').First();
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x =>
                x.EndsWith(".rs", StringComparison.CurrentCultureIgnoreCase) || x.EndsWith("Cargo.toml", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var packageName = "default_submission";
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.rs", SearchOption.AllDirectories);
            log.Information("{count} Rust Dateien zum kompelieren gefunden.", compilableFiles.Length);

            var binaryDate = GetLatestFileChange(paths.Host.BinaryPath);
            if (!forceRecompile && compilableFiles.All(x => File.GetLastWriteTime(x) < binaryDate))
            {
                log.Information("Überspringe Kompilierung, da schon ein aktuelles Binary vorliegt");
            }
            else
            {
                PrepareCompilation(paths, compilableFiles, packageName);
                Compile(paths, processProvider);
            }

            return new ExecutionParameters
            {
                Path = $"{paths.Sandbox.BinaryPath}/debug/{packageName}",
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Arguments = new string[0],
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }

        public void PrepareCompilation(CompilerPaths paths, IEnumerable<string> compilableFiles, string packageName)
        {
            CleanOutputBin(paths);

            var basePath = paths.Host.SourcePath;
            var cargoPath = GetCargoPath(paths);
            Cargo cargo = null;
            if (cargoPath != null)
            {
                cargo = new Cargo(File.ReadAllText(cargoPath));
            }
            else
            {
                var mainFile = GetMainFile(compilableFiles);
                File.Move(mainFile, Path.Combine(basePath, "main.rs"));
                var allFiles = Directory.GetFiles(basePath, "*.rs", SearchOption.AllDirectories).Select(p => p[(basePath.Length + 1)..]);
                Directory.CreateDirectory(Path.Combine(basePath, "src"));
                foreach (var file in allFiles)
                {
                    var source = Path.Combine(basePath, file);
                    var dest = Path.Combine(basePath, "src", file);
                    File.Move(source, dest);
                }

                cargo = Cargo.GetDefaultCargo();
            }

            cargo.SetPackageName(packageName);
            File.WriteAllText(Path.Combine(paths.Host.SourcePath, "Cargo.toml"), cargo.GetText());
            var configDirectory = Path.Combine(paths.Host.SourcePath, ".cargo");
            Directory.CreateDirectory(configDirectory);

            //Is used to set the Output-Path
            File.WriteAllText(Path.Combine(configDirectory, "config"), $@"
[build]
target-dir = ""{paths.Sandbox.BinaryPath}""
");
        }

        protected void Compile(CompilerPaths paths, IProcessProvider processProvider)
        {
            ISyncLock compilelock = null;
            try
            {
                compilelock = processProvider.GetLock(new[]
                {
                    new FolderMapping {Source = paths.Host.SourcePath, Target = paths.Sandbox.SourcePath, ReadOnly = true},
                    new FolderMapping {Source = paths.Host.BinaryPath, Target = paths.Sandbox.BinaryPath}
                });

                var result = processProvider.Execute("cargo", new[] {"rustc", "--"}.Concat(CompilerSwitches).ToArray(), workingDir: paths.Sandbox.SourcePath,
                    syncLock: compilelock).Result;
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

        protected string GetCargoPath(CompilerPaths paths)
        {
            var result = Directory.GetFiles(paths.Host.SourcePath, "Cargo.toml", SearchOption.TopDirectoryOnly);
            if (result.Length == 1)
            {
                log.Information("Cargo found");
                return result[0];
            }

            log.Information($"{result.Length} Cargo founds");
            return null;
        }

        protected string GetMainFile(IEnumerable<string> compilableFiles)
        {
            string mainFile;
            if (compilableFiles.Count() == 1)
            {
                mainFile = compilableFiles.ElementAt(0);
            }
            else
            {
                var matching = compilableFiles.Where(x => Regex.IsMatch(File.ReadAllText(x), @"fn\s+main\(")).ToList();
                if (matching.Count == 1)
                {
                    mainFile = matching[0];
                }
                else if (matching.Count == 0)
                {
                    throw new CompilerException("Mehrere Rust-Dateien gefunden, aber keine der Dateien enthält fn main()", Name);
                }
                else
                {
                    throw new CompilerException("Mehrere Rust-Dateien gefunden, aber mehrere der Dateien enthalten eine Definition für fn main()", Name);
                }
            }

            return mainFile;
        }

        protected static DateTime GetLatestFileChange(string directory)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            return files.Length == 0 ? DateTime.MaxValue : files.Select(File.GetLastWriteTime).Max();
        }
    }
}
