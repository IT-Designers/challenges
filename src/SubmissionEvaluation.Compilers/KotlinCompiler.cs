using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Compilers;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

public class KotlinCompiler : CompilerBase
{
    private readonly string kotlinCompilerPath;
    private readonly string kotlinPath;
    private readonly ILog log;

    public KotlinCompiler(ILog log, string kotlinPath, string kotlinCompilerPath)
    {
        this.log = log;
        this.kotlinPath = kotlinPath;
        this.kotlinCompilerPath = kotlinCompilerPath;
        CompilerSwitches = new[] {"-include-runtime"};
    }

    public override string Name => "Kotlin";

    public override string Description => "";


    public override string LatexCodeExtension => ".kt";


    public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
    {
        var result = processProvider.Execute(kotlinCompilerPath, new[] {"-version"}, syncLock: versionlock);
        if (result.Result.ExitCode == 0)
        {
            return "Kotlin " + result.Result.Output;
        }

        throw new CompilerException("Compiler nicht verfügbar", Name);
    }

    public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
    {
        return files.Where(x => x.EndsWith(".kt", StringComparison.CurrentCultureIgnoreCase));
    }

    public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
    {
        var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.kt", SearchOption.AllDirectories);
        var jarfile = "submission.jar";
        var jarPath = $"{paths.Sandbox.BinaryPath}/{jarfile}";
        log.Information("{count} Kotlin Dateien gefunden.", compilableFiles.Length);

        var jarDate = File.GetLastWriteTime(jarPath);
        if (!forceRecompile && compilableFiles.All(x => File.GetLastWriteTime(x) < jarDate))
        {
            log.Information("Überspringe Kompilierung, da die Jar Datei aktuell ist.");
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
                var result = processProvider.Execute(kotlinCompilerPath, CompilerSwitches.Concat(new[] {"-d", jarPath}).Concat(files).ToArray(),
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
            Path = kotlinPath,
            Arguments = new[] {"-Dfile.encoding=UTF-8", "-jar", jarfile},
            WorkingDirectory = paths.Sandbox.BinaryPath,
            Language = Name,
            OutputEncoding = "UTF8"
        };
    }
}
