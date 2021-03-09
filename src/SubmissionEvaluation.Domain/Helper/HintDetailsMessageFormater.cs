using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Domain.Properties;

namespace SubmissionEvaluation.Domain.Helper
{
    internal static class HintDetailsMessageFormater
    {
        public static string FormatHintDetailsMessage(IEnumerable<FailedTestRunDetails> details)
        {
            var detail = details.FirstOrDefault();
            if (detail == null)
            {
                return string.Empty;
            }

            var message = new StringBuilder();
            message.Append("<div>");
            message.Append("<h4>Details für ersten fehlerhaften Testfall</h4>");
            var detailMessage = FormatHintDetail(detail);
            message.Append(detailMessage);
            message.Append("</div>");
            return message.ToString();
        }

        private static string FormatHintDetail(FailedTestRunDetails detail)
        {
            var message = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(detail.ErrorMessage))
            {
                message.AppendFormat(Resources.EvaluationMessages_BodyErrorMessage, detail.ErrorMessage);
            }

            if (!string.IsNullOrWhiteSpace(detail.HintMessage))
            {
                message.Append("<h5>" + Resources.EvaluationMessages_BodyHint + "</h5>");
                message.Append(detail.HintMessage);
            }

            if (detail.HintCategories != null)
            {
                foreach (var category in detail.HintCategories)
                {
                    if (category.Hints.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(category.Title))
                        {
                            message.AppendLine($"<h5>{HttpUtility.HtmlEncode(category.Title)}</h5>");
                        }

                        if (category.IsTabbed && category.Hints.Count > 1)
                        {
                            var prefix = category.GetHashCode().ToString();
                            message.AppendLine($"<ul class=\"nav nav-tabs\" id=\"tab_{prefix}\" role=\"tablist\">");
                            for (var i = 0; i < category.Hints.Count; i++)
                            {
                                var active = i == 0 ? " active" : "";
                                message.AppendLine("<li class=\"nav-item\">");
                                message.AppendLine(
                                    $"<a class=\"nav-link{active}\" id=\"tab_{prefix}_{i}\" data-toggle=\"tab\" href=\"#tr{prefix}_{i}\" role=\"tab\" aria-controls=\"tr{prefix}_{i}\" aria-selected=\"{i == 0}\">{category.Hints[i].Header}</a>");
                                message.AppendLine("</li>");
                            }

                            message.AppendLine("</ul>");
                            message.AppendLine($"<div class=\"tab-content\" id=\"tab_{prefix}Content\">");
                            for (var i = 0; i < category.Hints.Count; i++)
                            {
                                var hint = category.Hints[i];
                                var active = i == 0 ? " show active" : "";
                                message.AppendLine(
                                    $"<div class=\"tab-pane fade {active}\" id=\"tr{prefix}_{i}\" role=\"tabpanel\" aria-labelledby=\"tab_{prefix}_{i}\">");
                                if (hint.IsHtml)
                                {
                                    message.AppendLine(hint.Content);
                                }
                                else
                                {
                                    message.AppendLine($"<pre><code>{HttpUtility.HtmlEncode(hint.Content)}" + "</code></pre>");
                                }

                                message.AppendLine("</div>");
                            }

                            message.AppendLine("</figure></div>");
                        }
                        else
                        {
                            foreach (var hint in category.Hints)
                            {
                                message.AppendLine($"<h6>{HttpUtility.HtmlEncode(hint.Header)}</h6>");
                                if (hint.IsHtml)
                                {
                                    message.AppendLine(hint.Content);
                                }
                                else
                                {
                                    message.AppendLine($"<pre><code>{HttpUtility.HtmlEncode(hint.Content)}" + "</code></pre>");
                                }
                            }
                        }
                    }
                }
            }

            return message.ToString();
        }
    }
}
