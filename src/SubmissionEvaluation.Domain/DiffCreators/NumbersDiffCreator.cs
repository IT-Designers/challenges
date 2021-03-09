using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SubmissionEvaluation.Domain.DiffCreators
{
    internal class NumbersDiffCreator : IDiffCreator
    {
        private readonly double deltaThreshold;
        private readonly bool unifyFloatingNumbers;

        public NumbersDiffCreator(bool unifyFloatingNumbers, double deltaThreshold)
        {
            this.deltaThreshold = deltaThreshold;
            this.unifyFloatingNumbers = unifyFloatingNumbers;
        }

        public (bool Success, string Details, bool showExpected) GetDiff(string submission, string solution)
        {
            var submissionLines = Regex.Split(submission.Trim(), "\r\n|\n");
            var solutionLines = Regex.Split(solution.Trim(), "\r\n|\n");

            if (submissionLines.Length != solutionLines.Length)
            {
                return (false, $"Erwartet wurden {solutionLines.Length}, aber nur {submissionLines.Length} wurde ausgegeben.", true);
            }

            for (var i = 0; i < solutionLines.Length; i++)
            {
                var isLine = submissionLines[i];
                var shouldLine = solutionLines[i];
                if (unifyFloatingNumbers)
                {
                    isLine = isLine.Replace(',', '.');
                }

                if (!double.TryParse(isLine, NumberStyles.Any, CultureInfo.InvariantCulture, out var isNumber))
                {
                    return (false, $"{isLine} konnte nicht in eine Nummber konvertiert werden.", true);
                }

                if (!double.TryParse(shouldLine, NumberStyles.Any, CultureInfo.InvariantCulture, out var shouldNumber))
                {
                    return (false,
                        $"{shouldNumber} konnte nicht in eine Nummber konvertiert werden. Der Fehler liegt beim Ersteller der Aufgabe. Kontaktiere ihn oder einen Administartor.",
                        true);
                }

                if (Math.Abs(isNumber - shouldNumber) > deltaThreshold)
                {
                    return (false, $"{isNumber} is not equal with {shouldNumber} within the threshold.", true);
                }
            }

            return (true, "", true);
        }
    }
}
