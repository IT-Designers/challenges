using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Providers;

namespace SubmissionEvaluation.ChallengeEstimator
{
    public class ChallengeEstimator : IChallengeEstimator
    {
        public Effort GuessEffortFor(Challenge challenge, IEnumerable<Result> results, double? lastEstimationRating)
        {
            var efforts = new List<int>();
            var passed = results.Where(x => x.IsPassed).ToList();
            foreach (var result in passed.Where(x => x.Language == "C#"))
            {
                efforts.Add(GuessEffortForCs(result.SubmissionPath, challenge.State.DifficultyRating));
            }

            foreach (var result in passed.Where(x => x.Language == "Java"))
            {
                efforts.Add(GuessEffortForJava(result.SubmissionPath, challenge.State.DifficultyRating));
            }

            if (efforts.Count > 0)
            {
                var effortRating = Median(efforts);
                var rawRating = effortRating;

                if (lastEstimationRating.HasValue)
                {
                    effortRating -= lastEstimationRating.Value;
                }

                if (effortRating < 15)
                {
                    effortRating = (int) (Math.Ceiling(effortRating / 5.0) * 5);
                }
                else if (effortRating < 60)
                {
                    effortRating = (int) (Math.Ceiling(effortRating / 15.0) * 15);
                }
                else
                {
                    effortRating = (int) (Math.Ceiling(effortRating / 60.0) * 60);
                }

                if (effortRating < 5)
                {
                    effortRating = 5;
                }

                var minduration = effortRating < 60 ? $"{effortRating} mins" : $"{Math.Ceiling(effortRating / 60.0)} hrs";
                var maxduration = effortRating * 3 < 60 ? $"{effortRating * 3} mins" : $"{Math.Ceiling(effortRating * 3 / 60.0)} hrs";
                return new Effort {Min = minduration, Max = maxduration, Rating = effortRating, RawRating = rawRating};
            }

            return new Effort {Min = "30 mins", Max = "8 hrs", Rating = 0};
        }

        public List<string> FindFeaturesFor(IEnumerable<Result> results)
        {
            var workingSubmissionCount = 0;
            var features = new List<string>();
            foreach (var result in results.Where(x => x.IsPassed && x.Language == "C#"))
            {
                features.AddRange(FindFeaturesFor(result.SubmissionPath));
                workingSubmissionCount++;
            }

            if (workingSubmissionCount < 3)
            {
                return new List<string>();
            }

            var featureGroups = features.GroupBy(x => x);
            return featureGroups.Where(x => x.Count() > workingSubmissionCount / 3).Select(x => x.Key).OrderBy(x => x).ToList();
        }

        public double Median(List<int> numbers)
        {
            if (numbers.Count == 0)
            {
                return double.NaN;
            }

            var sortedNumbers = numbers.OrderBy(n => n).ToList();
            var numberCount = numbers.Count;
            var halfIndex = numbers.Count / 2;
            if (numberCount % 2 == 0)
            {
                return (sortedNumbers[halfIndex - 1] + sortedNumbers[halfIndex]) / 2.0;
            }

            return sortedNumbers[halfIndex];
        }

        private static int GuessEffortForCs(string path, int? difficulty)
        {
            var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);
            var linecount = files.Select(x => File.ReadLines(x).Count()).Sum();
            return (int) (linecount / 9.0 * ((difficulty ?? 40) / 40.0));
        }

        private static int GuessEffortForJava(string path, int? difficulty)
        {
            var files = Directory.EnumerateFiles(path, "*.java", SearchOption.AllDirectories);
            var linecount = files.Select(x => File.ReadLines(x).Count()).Sum();
            return (int) (linecount / 10.0 * ((difficulty ?? 40) / 40.0));
        }

        private static IEnumerable<string> FindFeaturesFor(string path)
        {
            var files = Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories);
            return files.SelectMany(FindFeaturesInSourcefile).Distinct();
        }

        private static IEnumerable<string> FindFeaturesInSourcefile(string file)
        {
            var source = File.ReadAllText(file);
            var tree = CSharpSyntaxTree.ParseText(source);
            return FindFeaturesInTree(tree);
        }

        private static IEnumerable<string> FindFeaturesInTree(SyntaxTree tree)
        {
            var creations = tree.GetRoot().DescendantNodes().OfType<ObjectCreationExpressionSyntax>();
            var creationNames = creations.Select(x => x.Type.ToFullString()).ToList();
            var invocations = tree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>();
            var invocationNames = invocations.Select(x => x.Expression.ToString().Split('.').Last()).ToList();

            var results = new List<string>();
            AddIfArraysUsed(tree, results);
            AddIfCollectionsUsed(creationNames, results);
            AddIfDictionariesUsed(creationNames, results);
            AddIfStringOperationsUsed(invocationNames, results);
            AddIfIterationsUsed(tree, results);
            AddIfFileOperationsUsed(invocationNames, results);
            return results;
        }


        private static void AddIfArraysUsed(SyntaxTree tree, List<string> results)
        {
            if (tree.GetRoot().DescendantNodes().OfType<ArrayCreationExpressionSyntax>().Any())
            {
                results.Add("Arrays");
            }
        }

        private static void AddIfCollectionsUsed(List<string> creationNames, List<string> results)
        {
            if (creationNames.Any(x => x.StartsWith("List<") || x.StartsWith("System.Collections.Generic.List<")))
            {
                results.Add("Collections");
            }
        }

        private static void AddIfDictionariesUsed(List<string> creationNames, List<string> results)
        {
            if (creationNames.Any(x => x.StartsWith("Dictionary<") || x.StartsWith("System.Collections.Generic.Dictionary<")))
            {
                results.Add("Hashtables");
            }
        }

        private static void AddIfStringOperationsUsed(IEnumerable<string> invocationNames, List<string> results)
        {
            var stringOpsList = new List<string>
            {
                "Substring",
                "IndexOf",
                "Split",
                "ToLower",
                "ToUpper"
            };
            if (invocationNames.Any(x => stringOpsList.Contains(x)))
            {
                results.Add("String Operations");
            }
        }

        private static void AddIfIterationsUsed(SyntaxTree tree, List<string> results)
        {
            var forStatements = tree.GetRoot().DescendantNodes().OfType<ForStatementSyntax>();
            var foreachStatements = tree.GetRoot().DescendantNodes().OfType<ForEachStatementSyntax>();
            var whileStatements = tree.GetRoot().DescendantNodes().OfType<WhileStatementSyntax>();
            if (forStatements.Any() || foreachStatements.Any() || whileStatements.Any())
            {
                results.Add("Iterations");
            }
        }

        private static void AddIfFileOperationsUsed(IEnumerable<string> invocationNames, List<string> results)
        {
            var stringOpsList = new List<string>
            {
                "ReadAllText",
                "ReadAllLines",
                "ReadLines",
                "AppendAllLines",
                "AppendAllText",
                "AppendText",
                "OpenRead",
                "OpenWrite",
                "OpenText",
                "WriteAllText",
                "WriteAllLines"
            };
            if (invocationNames.Any(x => stringOpsList.Contains(x)))
            {
                results.Add("File Operations");
            }
        }
    }
}
