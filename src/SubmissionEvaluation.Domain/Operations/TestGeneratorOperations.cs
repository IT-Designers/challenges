using System;
using System.Linq;
using System.Text.RegularExpressions;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Exceptions;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.Domain.Operations
{
    internal class TestGeneratorOperations
    {
        public ILog Log;
        public IProcessProvider SandboxedProcessProvider { private get; set; }
        public IFileProvider FileProvider { get; set; }
        public CompilerOperations CompilerOperations { get; set; }

        public string GenerateTest(Challenge challengeParameters, ISubmission submission, string input, string[] arguments)
        {
            try
            {
                var pathToZip = FileProvider.GetSourceZipPathForSubmission(submission);
                var (execParams, sizeInfo, submissionBinaryPath) = CompilerOperations.CompileSubmission(pathToZip, challengeParameters, false);
                return PrepareRunSubmission(challengeParameters, execParams, submissionBinaryPath, input, arguments);
            }
            catch (CompilerException e)
            {
                Log.Warning("Compilerfehler aufgetretten", e);
            }
            catch (LanguageNotAllowedException)
            {
                Log.Warning("Nicht erlaubte Programmiersprache für Aufgabe verwendet!");
            }
            catch (Exception e)
            {
                Log.Error("Fehler während des Kompilierens aufgetretten:\n{message}", e.ToString());
            }

            return "";
        }

        private string PrepareRunSubmission(IChallenge challenge, ExecutionParameters execParams, string submissionBinaryPath, string input, string[] arguments)
        {
            var folderMappings = new[] {new FolderMapping {ReadOnly = true, Source = submissionBinaryPath, Target = "/testrun/bin"}};

            ISyncLock CreateLock()
            {
                return SandboxedProcessProvider.GetLock(folderMappings.ToArray());
            }

            var lockObject = CreateLock();
            try
            {
                var output = Run(
                    new ExecutionParameters
                    {
                        Path = execParams.Path,
                        Arguments = execParams.Arguments.Concat(execParams.PreludingTestParameters).Concat(arguments).ToArray(),
                        WorkingDirectory = execParams.WorkingDirectory,
                        Language = execParams.Language,
                        OutputEncoding = execParams.OutputEncoding,
                        Env = execParams.Env,
                        TimeoutBonus = execParams.TimeoutBonus,
                        PutStdinToFile = execParams.PutStdinToFile,
                        OutputFilter = execParams.OutputFilter
                    }, input, lockObject);
                return output;
            }
            finally
            {
                SandboxedProcessProvider.ReleaseLock(lockObject);
            }
        }

        private string Run(ExecutionParameters execParams, string input, ISyncLock lockObject)
        {
            Log.Information("Starte Sandboxed Testgeneration {path} {args} in {dir}", execParams.Path, execParams.Arguments, execParams.WorkingDirectory);

            // Fix Newlines befor passing
            var inputFixed = input != null ? Regex.Replace(input, @"\r\n?|\n", "\n") : null;
            var timeout = 5;

            var processResult = SandboxedProcessProvider.Execute(execParams.Path, execParams.Arguments, inputFixed, execParams.WorkingDirectory,
                (timeout + execParams.TimeoutBonus) * 1000, execParams.OutputEncoding, null, execParams.Env, syncLock: lockObject).Result;
            if (processResult.Exception)
            {
                Log.Warning("Exception bei Programmausführung");
                throw new EvaluationException("Fehler bei der Ausführung. Bitte den Administrator oder Aufgabenersteller benachrichtigen.");
            }

            var resultOutput = processResult.Output;
            // HOTFIX: Fixing errors with some submissions
            resultOutput = resultOutput.Replace(Convert.ToChar(0x0).ToString(), "");
            return resultOutput;
        }
    }
}
