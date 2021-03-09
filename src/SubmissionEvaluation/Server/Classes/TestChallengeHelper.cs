using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Server.Classes.JekyllHandling;
using SubmissionEvaluation.Shared.Models.Shared;
using SubmissionEvaluation.Shared.Models.Test;

namespace SubmissionEvaluation.Server.Classes
{
    public class TestChallengeHelper
    {
        public static List<ChallengeTest> GetTests(IChallenge challengeProps, ILogger logger)
        {
            var testParameters = new List<ChallengeTest>();
            try
            {
                var index = 0;
                var tests = JekyllHandler.Domain.Query.GetTests(challengeProps);
                var oldRestore = false;
                foreach (var test in tests)
                {
                    if (test.Id == 0)
                    {
                        test.Id = index + 1;
                        oldRestore = true;
                    }

                    var challengeTest = new ChallengeTest
                    {
                        Index = index.ToString(),
                        Id = test.Id,
                        Input = test.Input?.Content,
                        Parameters = test.Parameters.ToList() ?? new List<string>(),
                        Hint = test.Hint,
                        Timeout = test.Timeout
                    };
                    if (test.ExpectedOutput != null)
                    {
                        challengeTest.Output.Content = test.ExpectedOutput.Content;
                        challengeTest.Output.Alternatives = test.ExpectedOutput.Alternatives;
                        challengeTest.Output.CompareSettings = ConvertToModelSettings(test.ExpectedOutput.Settings);
                    }

                    if (test.ExpectedOutputFile != null)
                    {
                        challengeTest.OutputFile = new OutputFile
                        {
                            Name = test.ExpectedOutputFile.Name,
                            Content = test.ExpectedOutputFile.Content,
                            CompareSettings = ConvertToModelSettings(test.ExpectedOutputFile.Settings)
                        };
                    }

                    if (test.InputFiles != null)
                    {
                        challengeTest.InputFiles = test.InputFiles.Select(x => ConvertToInputFile(x)).ToList();
                    }
                    else
                    {
                        challengeTest.InputFiles = new List<File>();
                    }

                    challengeTest.CustomTestRunnerName = test.CustomTestRunner?.Command;

                    testParameters.Add(challengeTest);
                    index++;
                }

                if (oldRestore)
                {
                    JekyllHandler.Domain.Interactions.UpdateTests(challengeProps, tests);
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning($"DataStore tried to load TestParameters for Challenge {challengeProps.Id} and failed. No testparams yet? Exception: " + ex);
            }

            return testParameters;
        }

        private static File ConvertToInputFile(FileDefinition definition)
        {
            return new File
            {
                Name = definition.Name, OriginalName = definition.ContentFile, LastModified = definition.LastModified?.ToString("dd.MM.yyyy HH:mm:ss")
            };
        }

        private static CompareSettingsModel ConvertToModelSettings(CompareSettings settings)
        {
            return new CompareSettingsModel
            {
                CompareMode = settings.CompareMode,
                Trim = settings.Trim,
                Whitespaces = settings.Whitespaces,
                IsUnifyFloatingNumbers = settings.UnifyFloatingNumbers,
                IsIncludeCase = settings.IncludeCase,
                IsKeepUmlauts = settings.KeepUmlauts,
                Threshold = settings.Threshold
            };
        }
    }
}
