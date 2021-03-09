using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Domain.Helper
{
    internal class HintDetailsGenerator
    {
        private static IEnumerable<string> EnquoteIfNecessary(string[] arguments)
        {
            return arguments.Select(x => x.StartsWith("\"") || !x.Contains(" ") ? x : "\"" + x + "\"");
        }

        public static List<HintCategory> GetHintDetails(RunResult result, int resultIndex, int resultCount, bool forceDetails)
        {
            var catTimeout = new HintCategory();
            var catTestDetails = new HintCategory {Title = "Testdetails", IsTabbed = true};
            var catOutput = new HintCategory {Title = "Bildschirmausgabe", IsTabbed = true};
            var catOutputFile = new HintCategory {Title = "Dateivergleich", IsTabbed = true};
            var catInputFiles = new HintCategory {Title = "Eingabedateien"};
            var catSetup = new HintCategory {Title = "Schritte vor Testausführung"};
            var details = new List<HintCategory>
            {
                catTimeout,
                catTestDetails,
                catOutput,
                catOutputFile,
                catInputFiles,
                catSetup
            };


            if (forceDetails || resultIndex <= 2 * resultCount / 3)
            {
                if (result.Timeout)
                {
                    catTimeout.Hints.Add(new HintDetail
                    {
                        Header = "Zeitüberschreitung (Timeout)",
                        IsHtml = true,
                        Content =
                            "<p>Die Testausführung wurde abgebrochen, da die Ausführung zu lange dauerte. Üblicherweiße liegt dies an einer fehlerhaften Implementierung (z.B. Endlosschleife). Bei manchen Aufgaben werden auch komplexere Algorithmen gefordert, die auf Performance optimiert werden müssen.</p>"
                    });
                }

                if (!string.IsNullOrWhiteSpace(result.TestParameters.Input?.Content))
                {
                    catTestDetails.Hints.Add(new HintDetail
                    {
                        Header = "Simulierte Tastatureingabe",
                        Filename = "input",
                        Content = Regex.Replace(result.TestParameters.Input.Content, @"\r\n?|\n", Environment.NewLine),
                        IsHtml = false
                    });
                }

                if (result.TestParameters.CustomTestRunner == null && result.TestParameters.Parameters?.Length > 0)
                {
                    catTestDetails.Hints.Add(new HintDetail
                    {
                        Header = "Startparameter (args)",
                        Filename = "Startparameter.txt",
                        Content = string.Join(" ", EnquoteIfNecessary(result.TestParameters.Parameters)),
                        IsHtml = false
                    });
                }

                AddDetailsForDiff("output", result.Diff, catOutput);
                AddDetailsForDiff(result.TestParameters.ExpectedOutputFile?.Name, result.FileDiff, catOutputFile);

                if (result.TestParameters.InputFiles?.Count > 0)
                {
                    foreach (var inputFile in result.TestParameters.InputFiles)
                    {
                        catInputFiles.Hints.Add(new HintDetail
                        {
                            Header = $"Dateiinhalt {inputFile.Name}",
                            Filename = inputFile.Name,
                            Content = File.ReadAllText(inputFile.ContentFilePath),
                            IsHtml = false
                        });
                    }
                }

                if (result.PreviousTestParemeters.Count > 0)
                {
                    var stepCounter = 1;
                    var previousSteps = "<h2>Setupschritte vor Testdurchführung</h2>";
                    foreach (var previous in result.PreviousTestParemeters)
                    {
                        previousSteps += $"<h3>Schritt {stepCounter}</h3>";
                        if (previous.Parameters?.Length > 0)
                        {
                            previousSteps +=
                                $"<h4>Startparemeter<h4><pre><code>{HttpUtility.HtmlEncode(string.Join(" ", EnquoteIfNecessary(previous.Parameters)))}</code></pre>{Environment.NewLine}";
                        }

                        if (!string.IsNullOrWhiteSpace(previous.Input?.Content))
                        {
                            previousSteps += "<h4>Simulierte Eingabe</h4><pre><code>" +
                                             HttpUtility.HtmlEncode(Regex.Replace(previous.Input.Content, @"\r\n?|\n", Environment.NewLine)) + "</code></pre>" +
                                             Environment.NewLine;
                        }

                        if (previous.InputFiles != null)
                        {
                            foreach (var inputFile in previous.InputFiles)
                            {
                                if (!string.IsNullOrWhiteSpace(inputFile?.ContentFilePath))
                                {
                                    previousSteps +=
                                        $"<h4>Dateiinhalt ({inputFile.Name})</h4><pre><code>{HttpUtility.HtmlEncode(File.ReadAllText(inputFile.ContentFilePath))}</code></pre>{Environment.NewLine}";
                                }
                            }
                        }

                        stepCounter++;
                    }

                    catSetup.Hints.Add(new HintDetail {Header = "Setupschritte", Filename = "Setupsteps.html", Content = previousSteps, IsHtml = true});
                }

                return details;
            }

            return details;
        }

        private static void AddDetailsForDiff(string name, ComparisonResult diff, HintCategory category)
        {
            if (diff?.Evaluated == true)
            {
                if (!string.IsNullOrWhiteSpace(diff.Difference))
                {
                    category.Hints.Add(new HintDetail
                    {
                        Header = "Differenz zwischen erwartet und bekommen", IsHtml = true, Filename = name + "_difference.html", Content = diff.Difference
                    });
                }

                if (diff.Output != null)
                {
                    var diffOutput = $"<pre><code>{diff.Output}</code></pre>" + Environment.NewLine +
                                     "<p><b>Hinweis:</b> Es werden nur die Programmausgaben aufgelistet. Benutzereingaben werden nicht angezeigt.</p>";
                    if (string.IsNullOrWhiteSpace(diff.Output))
                    {
                        diffOutput = $"<pre><code>{diff.Output}</code></pre>" + Environment.NewLine +
                                     "<p><b>Hinweis:</b> Das Programm hat nichts ausgegeben!</p>";
                    }

                    category.Hints.Add(
                        new HintDetail {Header = "Vom Programm bekommen", IsHtml = true, Filename = name + "_received.txt", Content = diffOutput});
                }

                if (diff.Expected != null)
                {
                    category.Hints.Add(new HintDetail
                    {
                        Header = "Vom Test erwartet", IsHtml = true, Filename = name + "_expected.txt", Content = diff.Expected
                    });
                }
            }
        }
    }
}
