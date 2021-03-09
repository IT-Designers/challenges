using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class FsCompiler : CompilerBase
    {
        private readonly ILog log;
        private readonly string pathtoDotNet;

        public FsCompiler(ILog log, string pathtoDotNet)
        {
            this.log = log;
            this.pathtoDotNet = pathtoDotNet;
        }

        public override string Name => "F#";

        public override string Description =>
            "Bei F# ist für die Auflösung von Abhängigkeiten die Reihenfolge der Dateinamen zu berücksichtigen. Die Sortierung erfolgt alphabetisch.";

        public override string LatexCodeExtension => ".fsharp";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(pathtoDotNet, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0 && !string.IsNullOrEmpty(result.Result.Output))
            {
                return result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".fs", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.fs", SearchOption.AllDirectories);
            log.Information("{count} F# Dateien zum kompelieren gefunden.", compilableFiles.Length);

            var projectFile = Directory.EnumerateFiles(paths.Host.SourcePath, "*.fsproj", SearchOption.AllDirectories).FirstOrDefault();
            if (projectFile == null)
            {
                projectFile = Path.Combine(paths.Host.SourcePath, "submission.fsproj");

                var sb = new StringBuilder(
                    "<Project Sdk=\"Microsoft.NET.Sdk\"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>netcoreapp2.0</TargetFramework></PropertyGroup><ItemGroup>");
                foreach (var file in compilableFiles)
                {
                    var srcFile =
                        $"{CompilerHelper.ConvertToSandboxPath(Path.Combine(paths.Sandbox.SourcePath, CompilerHelper.GetRelativePath(file, paths.Host.SourcePath)))}";
                    sb.Append($"<Compile Include=\"{srcFile}\" />");
                }

                sb.Append("</ItemGroup></Project>");
                File.WriteAllText(projectFile, sb.ToString());
            }

            var document = XDocument.Load(projectFile);
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace("empty", "http://demo.com/2011/demo-schema");
            var execName = document.XPathSelectElement("/Project/PropertyGroup/OutputType/TargetFramework", namespaceManager)?.Value;
            if (execName == null)
            {
                execName = Path.GetFileNameWithoutExtension(projectFile);
            }

            var binaryPath = $"{execName}.dll";

            var binaryDate = File.GetLastWriteTime(Path.Combine(paths.Host.BinaryPath, binaryPath));
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

                    var result = processProvider.Execute(pathtoDotNet, new[] {"publish", "-o", paths.Sandbox.BinaryPath, Path.GetFileName(projectFile)},
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

            return new ExecutionParameters
            {
                Path = "dotnet",
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Arguments = new[] {binaryPath},
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }
    }
}
