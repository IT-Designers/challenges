using System.Collections.Generic;
using NUnit.Framework;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Domain.RatingMethods;

namespace SubmissionEvaluationTest.Domain
{
    [TestFixture]
    public class TestRateFixed
    {
        [Test]
        public void IsNewSubmissionBetter_Should_Return_True_When_Newer_Is_Better()
        {
            var newSub = new SubmissionEntry {Points = 10};
            var oldSub = new SubmissionEntry {Points = 5};
            var rater = new RateFixed();
            var result = rater.Compare(newSub, oldSub);

            Assert.True(result > 0);
        }

        [Test]
        public void UpdatePoints_Should_Set_Same_Points_When_3_Submitter_Are_Passed()
        {
            var s1 = new SubmissionEntry();
            var s2 = new SubmissionEntry();
            var s3 = new SubmissionEntry();
            var submitters = new List<SubmissionEntry> {s1, s2, s3};

            var ret = new RateFixed();
            ret.UpdatePoints(submitters, new RatingPoints(0, 5, 0));

            Assert.AreEqual(5, s1.Points);
            Assert.AreEqual(5, s2.Points);
            Assert.AreEqual(5, s3.Points);
        }

        [Test]
        public void UpdateRanking_Should_Set_Rank_When_2_Submitters_Are_Passed()
        {
            var s1 = new SubmissionEntry();
            var s2 = new SubmissionEntry();
            var submitters = new List<SubmissionEntry> {s1, s2};

            var ret = new RateFixed();
            ret.UpdateRanking(submitters);

            Assert.AreEqual(1, s1.Rank);
            Assert.AreEqual(1, s2.Rank);
        }
    }
}
