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
    public class JavaMavenCompiler : CompilerBase
    {
        private const string DefaultVersion = "1.0";
        private const string DefaultArtifactId = "build";
        private readonly string javaPath;
        private readonly ILog log;
        private readonly string mvnPath;

        public JavaMavenCompiler(ILog log, string javaPath, string mvnPath)
        {
            this.javaPath = javaPath;
            this.mvnPath = mvnPath;
            this.log = log;
        }

        public override string Name => "Java";

        public override string Description =>
            @"#### Ein- und Ausgabe

Leider steht auf dem Challengesystem keine Console zur Verfuegung, da die Ein- und Ausgabe umgeleitet wird. Daher liefert `System.console()` immer `null` zurueck. Derzeit empfehlen wir stattdessen die Verwendung von `System.in` und `System.out` gegebenenfalls eingebettet in einen `BufferedReader` oder `Scanner`.
Beispielimplementierung:

```
import java.util.Scanner;

public final class Console {
    private Console() {
    }

    private static Scanner console = new Scanner(System.in);

    public static String readLine() {
        return console.nextLine();
    }

    public static void writeLine(String message) {
        System.out.println(message);
    }
}
```";

        public override string LatexCodeExtension => ".java";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var version = GetJavaVersion(processProvider, versionlock);
            version += " / " + GetMavenVersion(processProvider, versionlock);
            return version;
        }

        private string GetJavaVersion(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(javaPath, new[] {"-version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0 && !string.IsNullOrEmpty(result.Result.Output))
            {
                using var reader = new StringReader(result.Result.Output);
                return string.Join(" ", reader.ReadLine().Split(' ').Take(3));
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        private string GetMavenVersion(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(mvnPath, new[] {"-v"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0 && !string.IsNullOrEmpty(result.Result.Output))
            {
                using var reader = new StringReader(result.Result.Output);
                return string.Join(" ", reader.ReadLine().Split(' ').Take(3));
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.java", SearchOption.AllDirectories);

            log.Information("{count} JAVA Dateien zum kompelieren gefunden.", compilableFiles.Length);
            var latestChangeInBuildPath = GetLatestFileChange(paths.Host.BinaryPath);
            if (!forceRecompile && compilableFiles.All(x => File.GetLastWriteTime(x) < latestChangeInBuildPath))
            {
                log.Information("Überspringe Kompilierung, da keine Veränderungen vorliegen");
            }
            else
            {
                PrepareCompilation(paths, compilableFiles);
                Compile(paths, processProvider);
            }

            return new ExecutionParameters
            {
                Path = javaPath,
                Arguments = new[] {"-jar", $"./{DefaultArtifactId}-{DefaultVersion}.jar"}.ToArray(),
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8"
            };
        }

        private void PrepareCompilation(CompilerPaths paths, IEnumerable<string> compilableFiles)
        {
            CleanOutputBin(paths);
            var pomPath = GetPomPath(paths);
            Pom pom;
            if (pomPath != null)
            {
                pom = new Pom(File.ReadAllText(pomPath)) {Version = DefaultVersion, BuildDirectory = "../bin", ArtifactId = DefaultArtifactId};
            }
            else
            {
                var mavenBasePath = Path.Combine("src", "main", "java");
                var sourceWithInvalidPackage = compilableFiles
                    .Select(x => (currentPath: x, newPath: Path.Combine(paths.Host.SourcePath, mavenBasePath, GetPackagePath(x))))
                    .Where(x => x.newPath != Path.GetDirectoryName(x.currentPath)).ToList();
                if (sourceWithInvalidPackage.Count > 0)
                {
                    log.Information("Verschiebe {count} Quellcodes zu richtigem Package-Verz.", sourceWithInvalidPackage.Count);
                    foreach (var (currentPath, newPath) in sourceWithInvalidPackage)
                    {
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }

                        var pathMoveTo = Path.Combine(newPath, Path.GetFileName(currentPath));
                        if (!File.Exists(pathMoveTo))
                        {
                            File.Move(currentPath, pathMoveTo);
                        }
                    }
                }

                var main = FindMainClass(Directory.GetFiles(paths.Host.SourcePath, "*.java", SearchOption.AllDirectories));
                pom = Pom.GetDefaultPom(main);
            }

            File.WriteAllText(Path.Combine(paths.Host.SourcePath, "pom.xml"), pom.GetXMLString());
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

                var result = processProvider.Execute(mvnPath, new[] {"package"}, workingDir: paths.Sandbox.SourcePath, syncLock: compilelock,
                    timeout: 3 * 60 * 1000).Result;
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

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x =>
                x.EndsWith(".java", StringComparison.CurrentCultureIgnoreCase) || x.EndsWith("pom.xml", StringComparison.CurrentCultureIgnoreCase));
        }

        protected string GetPomPath(CompilerPaths paths)
        {
            var result = Directory.GetFiles(paths.Host.SourcePath, "pom.xml", SearchOption.TopDirectoryOnly);
            if (result.Length == 1)
            {
                log.Information("Pom found");
                return result[0];
            }

            log.Information($"{result.Length} Poms founds");
            return null;
        }

        protected static DateTime GetLatestFileChange(string directory)
        {
            var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
            return files.Length == 0 ? DateTime.MaxValue : files.Select(File.GetLastWriteTime).Max();
        }

        protected string GetPackagePath(string sourcefile)
        {
            const string searchPatternPackage = @"^\s*package\s+(?<package>.+);";
            var rgxPackage = new Regex(searchPatternPackage, RegexOptions.IgnoreCase);
            foreach (var line in File.ReadLines(sourcefile))
            {
                var match = rgxPackage.Match(line);
                if (match.Success)
                {
                    return match.Groups["package"].Value.Replace('.', Path.DirectorySeparatorChar);
                }
            }

            return "";
        }

        protected static string FindMainClass(string[] classes)
        {
            var outputClassName = "";
            const string searchPatternMain = @"\s^*?static\s+?(void|int)\s+?main\s*?\(\s*String.*\s*\)";
            const string searchPatternPackage = @"^\s*package\s+(?<package>.+);";
            var rgxMain = new Regex(searchPatternMain, RegexOptions.IgnoreCase);
            var rgxPackage = new Regex(searchPatternPackage, RegexOptions.IgnoreCase);
            var package = "";
            foreach (var className in classes)
            {
                package = "";
                using (var file = new StreamReader(className))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        var match = rgxPackage.Match(line);
                        if (match.Success)
                        {
                            package = match.Groups["package"].Value;
                        }

                        if (rgxMain.Matches(line).Count > 0)
                        {
                            outputClassName = className;
                            break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(outputClassName))
                {
                    break;
                }
            }

            if (outputClassName == "")
            {
                throw new CompilerException("Main Methode konnte nicht gefunden werden", "Java");
            }

            var packagePath = string.IsNullOrWhiteSpace(package)
                ? Path.GetFileNameWithoutExtension(outputClassName)
                : package + "." + Path.GetFileNameWithoutExtension(outputClassName);
            var directoryName = Path.GetDirectoryName(outputClassName);
            if (!string.IsNullOrWhiteSpace(package))
            {
                var packges = package.Split('.').Reverse();
                foreach (var pack in packges)
                {
                    if (directoryName?.EndsWith(pack) == true)
                    {
                        directoryName = directoryName.Remove(directoryName.Length - (pack.Length + 1));
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return packagePath;
        }
    }
}
