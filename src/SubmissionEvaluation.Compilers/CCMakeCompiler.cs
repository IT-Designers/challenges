using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Compilers.Helper;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class CcMakeCompiler : CompilerBase
    {
        private readonly string compilerPath;
        private readonly ILog log;
        private readonly string pathToCMake;
        private readonly string pathToCppCheck;

        public CcMakeCompiler(ILog log, string compilerPath, string pathToCppCheck, string pathToCMake)
        {
            this.log = log;
            this.compilerPath = compilerPath;
            this.pathToCppCheck = pathToCppCheck;
            this.pathToCMake = pathToCMake;
        }

        public override string Name => "C";

        public override string Description =>
            @"#### Funktionen wie gets und gets_s
Leider gibt es Probleme mit Funktionen wie `gets` und `gets_s`. Derzeit empfehlen wir stattdessen die Variante mit `fgets` und `Stdin` zu verwenden.

#### CMake
Für das Erstellen wird inzwischen CMake verwendet. Wenn keine Datei CMakelist vorhanden ist, wird eine Default-Datei erzeugt.
";

        public override string LatexCodeExtension => ".c";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var resultGcc = processProvider.Execute(compilerPath, new[] {"--version"}, syncLock: versionlock);
            var resultCmake = processProvider.Execute(pathToCMake, new[] {"--version"}, syncLock: versionlock);
            if (resultGcc.Result.ExitCode != 0)
            {
                throw new CompilerException("Compiler nicht verfügbar", Name);
            }

            if (resultCmake.Result.ExitCode != 0)
            {
                throw new CompilerException("Compiler nicht verfügbar. Da CMake fehlt.", Name);
            }

            using var readerGcc = new StringReader(resultGcc.Result.Output);
            using var readerCmake = new StringReader(resultCmake.Result.Output);
            return string.Join(" ", readerGcc.ReadLine().Split(' ').Take(3)) + " / " + readerCmake.ReadLine();
        }

        public override bool CanCompileContent(IEnumerable<string> files)
        {
            return files.Any(x => x.EndsWith(".c", StringComparison.CurrentCultureIgnoreCase));
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => (x.EndsWith(".c", StringComparison.CurrentCultureIgnoreCase) ||
                                     x.EndsWith(".h", StringComparison.CurrentCultureIgnoreCase) ||
                                     x.Equals("CMakeLists.txt", StringComparison.CurrentCultureIgnoreCase)) && !x.EndsWith("FixBufferIssue_Generated.c"));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            const string fixBufferIssueFileName = "FixBufferIssue_Generated.c";
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.c", SearchOption.AllDirectories);
            log.Information("{count} C Dateien zum kompelieren gefunden.", compilableFiles.Length);

            var cmakeList = LoadCMakelist(paths, compilableFiles);
            var executableName = cmakeList.GetExecutableNames().SingleOrDefault();
            if (executableName == null)
            {
                throw new CompilerException("More then one executable in CMakeLists.txt found", Name);
            }

            var existingBinary = Directory.GetFiles(paths.Host.BinaryPath, executableName).FirstOrDefault();
            var compile = true;
            if (existingBinary != null)
            {
                var binaryDate = File.GetLastWriteTime(Path.Combine(paths.Host.BinaryPath, existingBinary));
                if (compilableFiles.All(x => File.GetLastWriteTime(x) < binaryDate))
                {
                    compile = forceRecompile;
                }
            }

            var binaryPath = $"{paths.Sandbox.BinaryPath}/{executableName}";
            if (!compile)
            {
                log.Information("Überspringe Kompilierung, da schon ein aktuelles Binary vorliegt");
            }
            else
            {
                File.WriteAllText(Path.Combine(paths.Host.SourcePath, fixBufferIssueFileName),
                    "#include <stdio.h>\n__attribute__((__constructor__))\nvoid disableBuffer(void) { setbuf(stdout, NULL); }\n");
                CleanOutputBin(paths);
                cmakeList.AddSourceToExecutables(fixBufferIssueFileName);
                File.WriteAllText(Path.Combine(paths.Host.SourcePath, "CMakeLists.txt"), cmakeList.GetContent());
                Compile(paths, processProvider);
            }

            return new ExecutionParameters
            {
                Path = binaryPath,
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Arguments = new string[0],
                Language = Name,
                OutputEncoding = cmakeList.GetEncoding()
            };
        }

        private CMakeList LoadCMakelist(CompilerPaths paths, IEnumerable<string> compilableFiles)
        {
            var files = compilableFiles.Select(x => CompilerHelper.ConvertToSandboxPath(CompilerHelper.GetRelativePath(x, paths.Host.SourcePath)));

            var cmakeListPath = GetCMakeListsPath(paths);
            CMakeList cmakeList;
            if (cmakeListPath != null)
            {
                cmakeList = new CMakeList(File.ReadAllText(cmakeListPath));
            }
            else
            {
                cmakeList = CMakeList.GetDefaultCMakeListForC(files.ToArray());
            }

            cmakeList.SetExecutableOutputPath("../../bin");
            return cmakeList;
        }

        private void Compile(CompilerPaths paths, IProcessProvider processProvider)
        {
            ISyncLock compilelock = null;
            try
            {
                compilelock = processProvider.GetLock(new[]
                {
                    new FolderMapping {Source = paths.Host.SourcePath, Target = paths.Sandbox.SourcePath, ReadOnly = true},
                    new FolderMapping {Source = paths.Host.BinaryPath, Target = paths.Sandbox.BinaryPath}
                });

                var resultPreProcessing = processProvider.Execute(pathToCMake, new[] {"-H.", "-Bbuild"},
                    workingDir: paths.Sandbox.SourcePath, syncLock: compilelock).Result;
                if (resultPreProcessing.Timeout)
                {
                    throw new CompilerException("Timout während Kompilierevorgang", resultPreProcessing, Name);
                }

                if (resultPreProcessing.ExitCode != 0)
                {
                    throw new CompilerException("Kompiliervorgang fehlgeschlagen", resultPreProcessing, Name);
                }


                var resultCompilation = processProvider.Execute(pathToCMake, new[] {"--build", "build"},
                    workingDir: paths.Sandbox.SourcePath, syncLock: compilelock).Result;
                if (resultCompilation.Timeout)
                {
                    throw new CompilerException("Timout während Kompilierevorgang", resultCompilation, Name);
                }

                if (resultCompilation.ExitCode != 0)
                {
                    throw new CompilerException("Kompiliervorgang fehlgeschlagen", resultCompilation, Name);
                }

                var resultCppCheck = processProvider.Execute(pathToCppCheck, new[] {"--quiet", "--error-exitcode=1", "."}, workingDir: paths.Sandbox.SourcePath,
                    syncLock: compilelock).Result;
                if (resultCppCheck.ExitCode == 1)
                {
                    throw new CompilerException("Kompiliervorgang fehlgeschlagen", resultCppCheck, Name);
                }
            }
            finally
            {
                processProvider.ReleaseLock(compilelock);
            }
        }

        private string GetCMakeListsPath(CompilerPaths paths)
        {
            var result = Directory.GetFiles(paths.Host.SourcePath, "CMakeLists.txt", SearchOption.TopDirectoryOnly);
            if (result.Length == 1)
            {
                log.Information("CMakeList found");
                return result[0];
            }

            log.Information($"{result.Length} CMakeLists founds");
            return null;
        }
    }
}
