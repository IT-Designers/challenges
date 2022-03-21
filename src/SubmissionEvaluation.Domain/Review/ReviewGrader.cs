using System;
using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data.Review;

namespace SubmissionEvaluation.Domain.Review
{
    public class ReviewGrader
    {
        private readonly float[] ratingMarks = {0.0f, 1.0f, 1.3f, 1.7f, 2.0f, 2.3f, 2.7f, 3.0f, 3.3f, 3.7f, 4.0f, 4.3f, 4.7f, 5.0f, 5.3f, 5.7f, 6.0f};

        private readonly ReviewTemplate reviewTemplate;

        public ReviewGrader(ReviewTemplate reviewTemplate)
        {
            this.reviewTemplate = reviewTemplate;
        }

        public void GradeReview(ReviewData data)
        {
            //Get all Categories
            var categories = reviewTemplate.Categories.Select(p => p.Id);
            data.CategoryResults = categories.Select(categoryId => GenerateCetgoryResult(data, categoryId)).ToArray();
        }

        private ReviewCategoryResult GenerateCetgoryResult(ReviewData data, string categoryId)
        {
            var grade = RateCategory(data, categoryId);
            return new ReviewCategoryResult
            {
                CategoryId = categoryId, Grade = grade != 0f ? grade : (float?) null, CategoryComments = GenerateCategoryText(data, categoryId)
            };
        }

        private string GenerateCategoryText(ReviewData data, string categoryId)
        {
            var categoryIssues = GetIssueToCategory(categoryId);
            var gradesWithQuestions = categoryIssues.Select(p => (grade: GetGradeForIssue(p, data), p));
            var categoryText = "";
            foreach (var (grade, issue) in gradesWithQuestions)
            {
                if (grade == 4)
                {
                    if (!string.IsNullOrWhiteSpace(issue.Enough))
                    {
                        categoryText += issue.Enough += Environment.NewLine;
                    }
                }
                else if (grade == 6 && !string.IsNullOrWhiteSpace(issue.Bad))
                {
                    categoryText += issue.Bad += Environment.NewLine;
                }
            }

            return categoryText;
        }

        private float RateCategory(ReviewData data, string categoryId)
        {
            var categoryIssues = GetIssueToCategory(categoryId);

            var gradedWithQuantifier = categoryIssues.Select(p => (grade: GetGradeForIssue(p, data), quantifier: p.Quantifier));
            var filtered = gradedWithQuantifier.Where(p => p.grade > 0);
            //Sum up with Quantifier and divide

            var quantifier = 0;
            var mark = 0;
            foreach (var scores in filtered)
            {
                quantifier += scores.quantifier;
                mark += scores.quantifier * scores.grade;
            }

            float finalMark;
            if (quantifier != 0)
            {
                finalMark = mark / (float) quantifier;
            }
            else
            {
                finalMark = 0;
            }

            finalMark = GetClosedGrade(finalMark);
            return finalMark;
        }

        private float GetClosedGrade(float grade)
        {
            return ratingMarks.OrderBy(item => Math.Abs(grade - item)).First();
        }

        private int GetGradeForIssue(Issue issue, ReviewData data)
        {
            var (rating, issueCount) = GetRatingAndIssueCountForIssue(issue, data);

            if (issueCount == 0)
            {
                return rating == 0 ? 1 : 0;
            }

            return rating switch
            {
                0 => 1,
                1 => 4,
                2 => 6,
                3 => 0,
                _ => throw new Exception("This should not have happend.")
            };
        }

        private (int rating, int issueCount) GetRatingAndIssueCountForIssue(Issue issue, ReviewData data)
        {
            var rating = data.GuidedQuestionResult.Single(p => p.Issues.Any(q => q.Id == issue.Id)).Rating;
            var issueCount = data.GuidedQuestionResult.SelectMany(p => p.Issues).Single(p => p.Id == issue.Id).IssueCount;
            return (rating: rating ?? GuidedQuestion.NoEvaluation, issueCount);
        }

        private IEnumerable<Issue> GetIssueToCategory(string categoryId)
        {
            var categorieIssues = reviewTemplate.GuidedQuestions.SelectMany(p => p.Issues.Where(x => x.Category == categoryId));
            return categorieIssues;
        }
    }
}
