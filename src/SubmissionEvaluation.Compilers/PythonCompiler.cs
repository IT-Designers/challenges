using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Compilers
{
    public class PythonCompiler : CompilerBase
    {
        private readonly ILog log;
        private readonly string runtimePath;

        public PythonCompiler(ILog log, string runtimePath)
        {
            this.log = log;
            this.runtimePath = runtimePath;
        }

        public override string Name => "Python";

        public override string Description =>
            "Bei Encoding Problemen z. B. bei Umlauten. Sollte darauf geachtet werden, dass der Sourcecode als UTF-8 Datei eingereicht wird. Hierzu muss im Header: `# -*- coding: utf-8 -*-` angegeben werden. Leider scheint es dennoch nicht reibungslos zu klappen, über Tipps sind wir dankbar.";

        public override string LatexCodeExtension => ".py";

        public override string ReadVersionDetails(IProcessProvider processProvider, ISyncLock versionlock)
        {
            var result = processProvider.Execute(runtimePath, new[] {"--version"}, syncLock: versionlock);
            if (result.Result.ExitCode == 0)
            {
                return result.Result.Output;
            }

            throw new CompilerException("Compiler nicht verfügbar", Name);
        }

        public override IEnumerable<string> GetSourceFiles(IEnumerable<string> files)
        {
            return files.Where(x => x.EndsWith(".py", StringComparison.CurrentCultureIgnoreCase));
        }

        public override ExecutionParameters CompileContent(CompilerPaths paths, IProcessProvider processProvider, bool forceRecompile)
        {
            var env = new Dictionary<string, string> {{"PYTHONIOENCODING", "UTF-8"}};

            var compilableFiles = Directory.GetFiles(paths.Host.SourcePath, "*.py", SearchOption.AllDirectories);
            log.Information("{count} Python Dateien gefunden.", compilableFiles.Length);
            CleanOutputBin(paths);
            foreach (var file in compilableFiles)
            {
                var dest = Path.Combine(paths.Host.BinaryPath, CompilerHelper.GetRelativePath(file, paths.Host.SourcePath));
                var path = Path.GetDirectoryName(dest);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.Copy(file, dest, true);
            }

            string mainFile;
            if (compilableFiles.Length == 1)
            {
                mainFile = compilableFiles[0];
            }
            else
            {
                bool ContainsMain(string code)
                {
                    return code.Contains("if __name__ == \"__main__\":") || code.Contains("if __name__ == '__main__':");
                }

                var matching = compilableFiles.Where(x => ContainsMain(File.ReadAllText(x))).ToList();
                if (matching.Count == 1)
                {
                    mainFile = matching[0];
                }
                else if (matching.Count == 0)
                {
                    throw new CompilerException("Mehrere Python-Dateien gefunden, aber keine der Dateien enthält den Ausdruck if __name__ == \"__main__\":",
                        Name);
                }
                else
                {
                    throw new CompilerException(
                        "Mehrere Python-Dateien gefunden, aber mehrere der Dateien enthalten eine Definition für if __name__ == \"__main__\":", Name);
                }
            }

            var mainFileRel = CompilerHelper.GetRelativePath(mainFile, paths.Host.SourcePath);
            return new ExecutionParameters
            {
                Path = runtimePath,
                Arguments = new[] {mainFileRel},
                WorkingDirectory = paths.Sandbox.BinaryPath,
                Language = Name,
                OutputEncoding = "UTF8",
                Env = env
            };
        }
    }
}
