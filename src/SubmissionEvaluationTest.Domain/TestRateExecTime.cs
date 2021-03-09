using System.Collections.Generic;
using NUnit.Framework;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Domain.RatingMethods;

namespace SubmissionEvaluationTest.Domain
{
    [TestFixture]
    public class TestRateExecTime
    {
        [Test]
        public void IsNewSubmissionBetter_Should_Return_True_When_Newer_Is_Better()
        {
            var newSub = new SubmissionEntry {Exectime = 10};
            var oldSub = new SubmissionEntry {Exectime = 20};
            var rater = new RateExecTime();
            var result = rater.Compare(newSub, oldSub);

            Assert.True(result > 0);
        }

        [Test]
        public void UpdatePoints_Should_Set_Points_When_1_Submitter_Is_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var submitters = new List<SubmissionEntry> {s1};

            var ret = new RateExecTime();
            ret.UpdatePoints(submitters, new RatingPoints(0, 0, 10));

            Assert.AreEqual(10, s1.Points);
        }

        [Test]
        public void UpdatePoints_Should_Set_Points_When_2_Same_Submitters_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 100};
            var submitters = new List<SubmissionEntry> {s1, s2};

            var ret = new RateExecTime();
            ret.UpdatePoints(submitters, new RatingPoints(0, 0, 10));

            Assert.AreEqual(10, s1.Points);
            Assert.AreEqual(10, s2.Points);
        }

        [Test]
        public void UpdatePoints_Should_Set_Points_When_2_Submitters_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 200};
            var submitters = new List<SubmissionEntry> {s1, s2};

            var ret = new RateExecTime();
            ret.UpdatePoints(submitters, new RatingPoints(0, 5, 10));

            Assert.AreEqual(10, s1.Points);
            Assert.AreEqual(5, s2.Points);
        }

        [Test]
        public void UpdatePoints_Should_Set_Points_When_3_Submitters_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 200};
            var s3 = new SubmissionEntry {Exectime = 300};
            var submitters = new List<SubmissionEntry> {s1, s2, s3};

            var ret = new RateExecTime();
            ret.UpdatePoints(submitters, new RatingPoints(1, 5, 10));

            Assert.AreEqual(10, s1.Points);
            Assert.AreEqual(5, s2.Points);
            Assert.AreEqual(1, s3.Points);
        }

        [Test]
        public void UpdatePoints_Should_Set_Points_When_5_Submitters_with_3_same_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 200};
            var s3 = new SubmissionEntry {Exectime = 200};
            var s4 = new SubmissionEntry {Exectime = 200};
            var s5 = new SubmissionEntry {Exectime = 500};
            var submitters = new List<SubmissionEntry>
            {
                s1,
                s2,
                s3,
                s4,
                s5
            };

            var ret = new RateExecTime();
            ret.UpdatePoints(submitters, new RatingPoints(1, 5, 10));

            Assert.AreEqual(10, s1.Points);
            Assert.AreEqual(5, s2.Points);
            Assert.AreEqual(5, s3.Points);
            Assert.AreEqual(5, s4.Points);
            Assert.AreEqual(1, s5.Points);
        }

        [Test]
        public void UpdateRanking_Should_Set_Rank_When_1_Submitter_Is_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var submitters = new List<SubmissionEntry> {s1};

            var ret = new RateExecTime();
            ret.UpdateRanking(submitters);

            Assert.AreEqual(1, s1.Rank);
        }

        [Test]
        public void UpdateRanking_Should_Set_Rank_When_2_Submitters_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 200};
            var submitters = new List<SubmissionEntry> {s1, s2};

            var ret = new RateExecTime();
            ret.UpdateRanking(submitters);

            Assert.AreEqual(1, s1.Rank);
            Assert.AreEqual(2, s2.Rank);
        }

        [Test]
        public void UpdateRanking_Should_Set_Rank_When_4_Submitters_And_2_Same_Are_Passed()
        {
            var s1 = new SubmissionEntry {Exectime = 100};
            var s2 = new SubmissionEntry {Exectime = 200};
            var s3 = new SubmissionEntry {Exectime = 200};
            var s4 = new SubmissionEntry {Exectime = 400};
            var submitters = new List<SubmissionEntry> {s1, s2, s3, s4};

            var ret = new RateExecTime();
            ret.UpdateRanking(submitters);

            Assert.AreEqual(1, s1.Rank);
            Assert.AreEqual(2, s2.Rank);
            Assert.AreEqual(2, s3.Rank);
            Assert.AreEqual(4, s4.Rank);
        }
    }
}
