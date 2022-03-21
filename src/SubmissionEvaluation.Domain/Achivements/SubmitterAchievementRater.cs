using System;
using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;

namespace SubmissionEvaluation.Domain.Achivements
{
    public class SubmitterAchievementRater : IAchievementRater
    {
        private const string FirstSubmission = "beginner";
        private const string TwentyFiveWorkingSubmissions = "working";
        private const string FiftyWorkingSubmissions = "heavyWorking";
        private const string FiveWorkingSubmissionsInRow = "fiveInARow";
        private const string TwentySubmissionsInRow = "chainmaster";
        private const string GettingOld = "gettingOld";
        private const string FirstSolvedChallenge = "mountainClimber";
        private const string FirstSolvedLanguage = "hillClimber";
        private const string SolvedWithinDayOne = "dayOne";
        private const string NumberOne = "numberOne";
        private const string AllLanguagesSolved = "polyglot";
        private const string SemesterBest = "topPupil";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = FirstSubmission, Quality = QualityType.Bronze},
                new Achievement {Id = FiftyWorkingSubmissions, Quality = QualityType.Gold},
                new Achievement {Id = TwentyFiveWorkingSubmissions, Quality = QualityType.Silver},
                new Achievement {Id = FiveWorkingSubmissionsInRow, Quality = QualityType.Silver},
                new Achievement {Id = GettingOld, Quality = QualityType.Bronze},
                new Achievement {Id = TwentySubmissionsInRow, Quality = QualityType.Platin},
                new Achievement {Id = FirstSolvedChallenge, Quality = QualityType.Gold},
                new Achievement {Id = FirstSolvedLanguage, Quality = QualityType.Silver},
                new Achievement {Id = SolvedWithinDayOne, Quality = QualityType.Gold},
                new Achievement {Id = NumberOne, Quality = QualityType.Platin},
                new Achievement {Id = AllLanguagesSolved, Quality = QualityType.Silver},
                new Achievement {Id = SemesterBest, Quality = QualityType.Gold}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            AwardFirstSubmission(awards, conditions.GlobalRanklist);
            AwardXInRow(awards, conditions.Histories);
            AwardXWorkingSubmissions(awards, conditions.Rankings);
            AwardGettingOld(awards, conditions.Members);
            AwardFirstSolvedOnLevel(awards, conditions.ChallengeRanklists);
            AwardSolvedWithinDayOne(awards, conditions.Challenges, conditions.ChallengeRanklists);
            AwardNumberOne(awards, conditions.GlobalRanklist);
            AwardSemesterBest(awards, conditions.SemesterRanklists);
            AwardAllLanguagesSubmissions(awards, conditions.ChallengeRanklists, conditions.CompilerNames.ToList());
        }

        private void AwardNumberOne(Awards awards, GlobalRanklist globals)
        {
            foreach (var best in globals.Submitters.Where(x => x.Rank == 1))
            {
                if (best.SolvedCount > 10)
                {
                    awards.AwardWith(best.Id, NumberOne);
                }
            }
        }

        private void AwardSemesterBest(Awards awards, IEnumerable<GlobalRanklist> semesterRanklists)
        {
            foreach (var semesterRanklist in semesterRanklists)
            {
                if (semesterRanklist.CurrentSemester.LastDay <= DateTime.Now) //Means semester is already over
                {
                    var best = semesterRanklist.Submitters.Where(x => x.Rank == 1).FirstOrDefault(p => p.SolvedCount > 10);
                    if (best != null)
                    {
                        awards.AwardWith(best.Id, SemesterBest);
                    }
                }
            }
        }

        private void AwardSolvedWithinDayOne(Awards awards, IEnumerable<Challenge> challenges, IEnumerable<ChallengeRanklist> challengeRanklists)
        {
            foreach (var ranklist in challengeRanklists)
            {
                var challenge = challenges.SingleOrDefault(x => x.Id == ranklist.Challenge);
                if (challenge != null)
                {
                    var date = challenge.Date.Add(new TimeSpan(1, 23, 59, 59));
                    var fastSubmitters = ranklist.Submitters.Where(x => x.Date <= date && x.Points > 0);
                    foreach (var fast in fastSubmitters)
                    {
                        awards.AwardWith(fast.Id, SolvedWithinDayOne);
                    }
                }
            }
        }

        private void AwardFirstSolvedOnLevel(Awards awards, IEnumerable<ChallengeRanklist> challengeRanklists)
        {
            foreach (var ranklist in challengeRanklists)
            {
                var entries = ranklist.Submitters.Where(x => x.Points > 0).ToList();
                if (entries.Count <= 0)
                {
                    continue;
                }

                var firstSubmitter = entries.OrderBy(x => x.Date).First();
                awards.AwardWith(firstSubmitter.Id, FirstSolvedChallenge);

                var languages = entries.GroupBy(x => x.Language);
                foreach (var language in languages)
                {
                    var firstSubmitterPerLang = language.OrderBy(x => x.Date).First();
                    if (firstSubmitterPerLang.Id != firstSubmitter.Id)
                    {
                        awards.AwardWith(firstSubmitterPerLang.Id, FirstSolvedLanguage);
                    }
                }
            }
        }


        public void AwardXWorkingSubmissions(Awards awards, IEnumerable<SubmitterRankings> rankings)
        {
            foreach (var submitter in rankings)
            {
                var count = submitter.Submissions.Where(x => x.Rank > 0).Select(x => x.Challenge).Distinct().Count();
                if (count >= 50)
                {
                    awards.AwardWith(submitter.Name, FiftyWorkingSubmissions);
                }

                if (count >= 25)
                {
                    awards.AwardWith(submitter.Name, TwentyFiveWorkingSubmissions);
                }
            }
        }

        public void AwardXInRow(Awards awards, IEnumerable<SubmitterHistory> histories)
        {
            foreach (var history in histories)
            {
                var usedChallenges = new HashSet<string>();
                var challenges = new HashSet<string>();
                foreach (var entry in history.Entries.Where(x => x.Type == HistoryType.ChallengeSubmission).OrderBy(x => x.Date))
                {
                    if (entry.IsPassed && !usedChallenges.Contains(entry.Challenge) && entry.Date >= DateTime.Today.AddMonths(-12))
                    {
                        challenges.Add(entry.Challenge);
                    }
                    else
                    {
                        challenges.Clear();
                    }

                    usedChallenges.Add(entry.Challenge);
                    if (challenges.Count >= 5)
                    {
                        awards.AwardWith(history.Id, FiveWorkingSubmissionsInRow);
                    }

                    if (challenges.Count >= 20)
                    {
                        awards.AwardWith(history.Id, TwentySubmissionsInRow);
                        break;
                    }
                }
            }
        }

        public void AwardFirstSubmission(Awards awards, GlobalRanklist globals)
        {
            foreach (var submitter in globals.Submitters.Where(x => x.SubmissionCount > 0))
            {
                awards.AwardWith(submitter.Id, FirstSubmission);
            }
        }

        public void AwardGettingOld(Awards awards, IEnumerable<IMember> members)
        {
            var sorted = members.OrderBy(x => x.DateOfEntry).ToList();
            for (var i = 0; i < sorted.Count - 4; i++)
            {
                awards.AwardWith(sorted[i].Id, GettingOld);
            }
        }

        private void AwardAllLanguagesSubmissions(Awards awards, IEnumerable<ChallengeRanklist> challengeRanklists, IReadOnlyList<string> availableLanguages)
        {
            foreach (var challenge in challengeRanklists)
            {
                foreach (var submitter in challenge.Submitters)
                {
                    var languages = new List<string> {submitter.Language};
                    if (submitter.MoreLanguages != null)
                    {
                        languages.AddRange(submitter.MoreLanguages);
                    }

                    if (availableLanguages.All(p => languages.Contains(p)))
                    {
                        awards.AwardWith(submitter.Id, AllLanguagesSolved);
                    }
                }
            }
        }
    }
}
