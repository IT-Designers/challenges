using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;
using SubmissionEvaluation.Domain.Achievements;

namespace SubmissionEvaluationTest.Domain
{
    [TestFixture]
    public class TestSubmitterAchievementRater
    {
        [Test]
        public void AddAwards_Should_Add_Achievement_For_First_Submission()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            var globalRanklist = new GlobalRanklist {Submitters = new List<GlobalSubmitter>()};
            globalRanklist.Submitters.Add(new GlobalSubmitter {Id = "123", SubmissionCount = 1});
            rater.AwardFirstSubmission(awards, globalRanklist);

            Assert.Contains(new Award {Id = "beginner"}, awards["123"].ToList());
        }

        [Test]
        public void AddAwards_Should_Add_Achievement_For_Five_In_A_Row()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            var submitterHistories = new List<SubmitterHistory>
            {
                new SubmitterHistory
                {
                    Id = "123",
                    Entries = new List<HistoryEntry>
                    {
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "A", Date = DateTime.Today},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "B", Date = DateTime.Today},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "C", Date = DateTime.Today},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "D", Date = DateTime.Today},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "E", Date = DateTime.Today}
                    }
                }
            };
            rater.AwardXInRow(awards, submitterHistories);

            Assert.Contains(new Award {Id = "fiveInARow"}, awards["123"].ToList());
        }

        [Test]
        public void AddAwards_Should_Add_Achievement_For_TwentyFive_Working_Submissions()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();

            var submitterRankings = new List<SubmitterRankings>
            {
                new SubmitterRankings
                {
                    Name = "123",
                    Submissions = new List<SubmitterRankingEntry>
                    {
                        new SubmitterRankingEntry {Rank = 1, Challenge = "A"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "B"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "C"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "D"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "E"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "F"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "G"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "H"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "I"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "J"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "K"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "L"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "M"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "N"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "O"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "P"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "Q"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "R"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "S"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "T"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "U"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "V"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "W"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "X"},
                        new SubmitterRankingEntry {Rank = 1, Challenge = "Y"}
                    }
                }
            };
            rater.AwardXWorkingSubmissions(awards, submitterRankings);

            Assert.Contains(new Award {Id = "working"}, awards["123"].ToList());
        }

        [Test]
        public void AddAwards_Should_Give_Achievement_For_Being_Longer_Member_Then_Five_Others()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            awards.AwardWith("1", "Nonsens");

            var members = new List<Member>
            {
                new Member {Id = "1", DateOfEntry = new DateTime(2000, 1, 1)},
                new Member {Id = "2", DateOfEntry = new DateTime(2001, 2, 1)},
                new Member {Id = "3", DateOfEntry = new DateTime(2002, 3, 1)},
                new Member {Id = "4", DateOfEntry = new DateTime(2003, 4, 1)},
                new Member {Id = "5", DateOfEntry = new DateTime(2004, 5, 1)}
            };
            rater.AwardGettingOld(awards, members);

            Assert.Contains(new Award {Id = "gettingOld"}, awards["1"].ToList());
        }

        [Test]
        public void AddAwards_Should_Not_Add_Achievement_For_Five_In_A_Row_With_Same_Challenges()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            awards.AwardWith("123", "Nonsens");
            var submitterHistories = new List<SubmitterHistory>
            {
                new SubmitterHistory
                {
                    Id = "123",
                    Entries = new List<HistoryEntry>
                    {
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "A"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "B"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "C"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "D"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "A"}
                    }
                }
            };
            rater.AwardXInRow(awards, submitterHistories);

            Assert.IsFalse(awards["123"].Contains(new Award {Id = "fiveInARow"}));
        }

        [Test]
        public void AddAwards_Should_Not_Give_Achievement_For_Five_In_A_Row()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            awards.AwardWith("123", "Nonsens");
            var submitterHistories = new List<SubmitterHistory>
            {
                new SubmitterHistory
                {
                    Id = "123",
                    Entries = new List<HistoryEntry>
                    {
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "A"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "B"},
                        new HistoryEntry {Result = EvaluationResult.TestsFailed, Challenge = "C"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "D"},
                        new HistoryEntry {Result = EvaluationResult.Succeeded, Challenge = "E"}
                    }
                }
            };
            rater.AwardXInRow(awards, submitterHistories);

            Assert.IsFalse(awards["123"].Contains(new Award {Id = "fiveInARow"}));
        }

        [Test]
        public void AddAwards_Should_Not_Give_Achievement_For_TwentyFive_Working_Submissions()
        {
            var rater = new SubmitterAchievementRater();
            var awards = new Awards();
            awards.AwardWith("123", "Nonsens");
            var submitterRankings = new List<SubmitterRankings>
            {
                new SubmitterRankings
                {
                    Name = "123",
                    Submissions = new List<SubmitterRankingEntry>
                    {
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1}
                    }
                },
                new SubmitterRankings
                {
                    Name = "456",
                    Submissions = new List<SubmitterRankingEntry>
                    {
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1},
                        new SubmitterRankingEntry {Rank = 1}
                    }
                }
            };
            rater.AwardXWorkingSubmissions(awards, submitterRankings);

            Assert.IsFalse(awards["123"].Contains(new Award {Id = "working"}));
        }
    }
}
