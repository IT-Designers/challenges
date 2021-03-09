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
    public class PerlCompiler : CompilerBase
    {
        private readonly ILog log;

        private readonly string runtimePath;

        public PerlCompiler(ILog log, string runtimePath)
        {
            this.log = log;
            this.runtimePath = runtimePath;
        }

        public override string LatexCodeExtension => ".pl";

        public override string Name => "Perl";

        public override string Description =>
            "Eine Einreichung darf nur genau eine Datei mit der .pl Endung enthalten. Sonst kann der Einstiegspunk nicht genau definiert werden.";


        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = GetPerlFiles(paths.Host.SourcePath);
            log.Information($"{compilableFiles.Length} Perl Dateien gefunden.");
            CleanOutputBin(paths);
            CopyFilesToBuiltDirectory(compilableFiles, paths.Host.BinaryPath, paths.Host.SourcePath);
            // first .pl file is selected as main file.
            var mainFile = CompilerHelper.ConvertToSandboxPath(Path.Combine(paths.Sandbox.BinaryPath,
                CompilerHelper.GetRelativePath(GetMainFile(compilableFiles), paths.Host.SourcePath)));
            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {mainFile},
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(runtimePath, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                var match = Regex.Match(result.Result.Output, "This is (?<version>.+) built for", RegexOptions.Singleline);
                if (match.Success)
                {
                    return match.Groups["version"].Value;
                }

                return "Perl";
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(IsPerlFile);
        }

        private static string GetMainFile(string[] compilableFiles)
        {
            return compilableFiles.First(f => f.EndsWith(".pl"));
        }

        private bool IsPerlFile(string file)
        {
            return file.EndsWith(".pl", StringComparison.CurrentCultureIgnoreCase) || file.EndsWith(".pm", StringComparison.CurrentCultureIgnoreCase);
        }

        private string[] GetPerlFiles(string pathToContent)
        {
            var filters = new[] {"*.pl", "*.pm"};
            var files = filters.SelectMany(f => Directory.GetFiles(pathToContent, f, SearchOption.AllDirectories));
            return files.ToArray();
        }

        private void CopyFilesToBuiltDirectory(string[] compilableFiles, string buildPath, string pathToContent)
        {
            foreach (var file in compilableFiles)
            {
                CopyFileToBuiltDirectory(file, buildPath, pathToContent);
            }
        }

        private void CopyFileToBuiltDirectory(string file, string buildPath, string pathToContent)
        {
            var path = Path.Combine(buildPath, CompilerHelper.GetRelativePath(file, pathToContent));
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.Copy(file, path, true);
        }
    }
}
