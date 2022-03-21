using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class ScalaCompiler : CompilerBase
    {
        private readonly string compilerPath;
        private readonly ILog log;
        private readonly string runtimePath;

        public ScalaCompiler(ILog log, string runtimePath, string compilerPath)
        {
            this.log = log;
            this.compilerPath = compilerPath;
            this.runtimePath = runtimePath;
        }

        public override string Name => "Scala";
        public override string Description => "-";
        public override string LatexCodeExtension => ".scala";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(runtimePath, new[] {"-version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output.Split(new[] {"--"}, StringSplitOptions.RemoveEmptyEntries).First();
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".scala", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var env = new Dictionary<string, string> {{"JAVA_TOOL_OPTIONS", "-Dfile.encoding=UTF8"}};
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.scala", SearchOption.AllDirectories);
            log.Information("{count} Scala Dateien zum kompelieren gefunden.", compilableFiles.Length);

            var latestChangeInBuildPath = GetLatestFileChange(paths.Host.BinaryPath);
            if (!forceRecompile && compilableFiles.All(x => File.GetLastWriteTime(x) < latestChangeInBuildPath))
            {
                log.Information("Überspringe Kompilierung, da keine Veränderungen vorliegen");
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
                    var result = processProvider.Execute(compilerPath, new[] {"-d", paths.Sandbox.BinaryPath}.Concat(files).ToArray(),
                        workingDir: paths.Sandbox.SourcePath, env: env, syncLock: compilelock).Result;
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

            var classname = FindMainClass(compilableFiles);
            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {classname},
                WorkingDirectory = paths.Sandbox.BinaryPath,
                TimeoutBonus = 3,
                Language = Name,
                OutputEncoding = "UTF8", // Requires JAVA_TOOL_OPTIONS='-Dfile.encoding=UTF8'
                Env = env,
                OutputFilter = new List<string> {"Picked up JAVA_TOOL_OPTIONS: -Dfile.encoding=UTF8" + Environment.NewLine}
            };
        }

        private static DateTime GetLatestFileChange(string directory)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            return files.Length == 0 ? DateTime.MaxValue : files.Select(File.GetLastWriteTime).Max();
        }

        private static string FindMainClass(string[] classes)
        {
            const string searchPatternAppClass = @"object (?<class>\w+) extends App(lication)? {";
            const string searchPatternClass = @"object (?<class>\w+) {";
            const string searchPatternMain = @"def\s*main\(.*\)((\s*:\s*Unit)?\s*=)?\s*\{";
            const string searchPatternPackage = @"^\s*package\s+(?<package>.+)$";
            var rgxAppCls = new Regex(searchPatternAppClass, RegexOptions.IgnoreCase);
            var rgxCls = new Regex(searchPatternClass, RegexOptions.IgnoreCase);
            var rgxMain = new Regex(searchPatternMain, RegexOptions.IgnoreCase);
            var rgxPackage = new Regex(searchPatternPackage, RegexOptions.IgnoreCase);
            foreach (var className in classes)
            {
                using var file = new StreamReader(className);
                var package = string.Empty;
                var cls = string.Empty;
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var packageMatch = rgxPackage.Match(line);
                    if (packageMatch.Success)
                    {
                        package = packageMatch.Groups["package"].Value;
                    }

                    var matchApp = rgxAppCls.Match(line);
                    if (matchApp.Success)
                    {
                        if (string.IsNullOrWhiteSpace(package))
                        {
                            return matchApp.Groups["class"].Value;
                        }

                        return package + "." + matchApp.Groups["class"].Value;
                    }

                    var matchMain = rgxMain.Match(line);
                    if (matchMain.Success)
                    {
                        if (string.IsNullOrWhiteSpace(package))
                        {
                            return cls;
                        }

                        return package + "." + cls;
                    }

                    var matchCls = rgxCls.Match(line);
                    if (matchCls.Success)
                    {
                        cls = matchCls.Groups["class"].Value;
                    }
                }
            }

            throw new CompilerException("Application Class bzw. def main() konnte nicht gefunden werden", "Scala");
        }
    }
}
