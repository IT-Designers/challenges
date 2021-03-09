using System;
using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Contracts.Data.Ranklist;

namespace SubmissionEvaluation.Domain.Achievements
{
    public class SubmitterAchievementRater : IAchievementRater
    {
        private const string firstSubmission = "beginner";
        private const string twentyFiveWorkingSubmissions = "working";
        private const string fiftyWorkingSubmissions = "heavyWorking";
        private const string fiveWorkingSubmissionsInRow = "fiveInARow";
        private const string twentySubmissionsInRow = "chainmaster";
        private const string gettingOld = "gettingOld";
        private const string firstSolvedChallenge = "mountainClimber";
        private const string firstSolvedLanguage = "hillClimber";
        private const string solvedWithinDayOne = "dayOne";
        private const string numberOne = "numberOne";
        private const string allLanguagesSolved = "polyglot";
        private const string semesterBest = "topPupil";

        public List<Achievement> ListOfAchievements =>
            new List<Achievement>
            {
                new Achievement {Id = firstSubmission, Quality = QualityType.Bronze},
                new Achievement {Id = fiftyWorkingSubmissions, Quality = QualityType.Gold},
                new Achievement {Id = twentyFiveWorkingSubmissions, Quality = QualityType.Silver},
                new Achievement {Id = fiveWorkingSubmissionsInRow, Quality = QualityType.Silver},
                new Achievement {Id = gettingOld, Quality = QualityType.Bronze},
                new Achievement {Id = twentySubmissionsInRow, Quality = QualityType.Platin},
                new Achievement {Id = firstSolvedChallenge, Quality = QualityType.Gold},
                new Achievement {Id = firstSolvedLanguage, Quality = QualityType.Silver},
                new Achievement {Id = solvedWithinDayOne, Quality = QualityType.Gold},
                new Achievement {Id = numberOne, Quality = QualityType.Platin},
                new Achievement {Id = allLanguagesSolved, Quality = QualityType.Silver},
                new Achievement {Id = semesterBest, Quality = QualityType.Gold}
            };

        public void AddAwards(Awards awards, AchievementConditions conditions)
        {
            AwardFirstSubmission(awards, conditions.GlobalRanklist);
            AwardXInRow(awards, conditions.Histories);
            AwardXWorkingSubmissions(awards, conditions.Rankings);
            AwardGettingOld(awards, conditions.Members);
            AwardFirstSolvedOnLevel(awards, conditions.Challenges, conditions.ChallengeRanklists);
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
                    awards.AwardWith(best.Id, numberOne);
                }
            }
        }

        private void AwardSemesterBest(Awards awards, IEnumerable<GlobalRanklist> semesterRanklists)
        {
            foreach (var semesterRanklist in semesterRanklists)
            {
                if (semesterRanklist.CurrentSemester.LastDay <= DateTime.Now) //Means semester is already over
                {
                    var best = semesterRanklist.Submitters.Where(x => x.Rank == 1).Where(p => p.SolvedCount > 10).FirstOrDefault();
                    if (best != null)
                    {
                        awards.AwardWith(best.Id, semesterBest);
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
                        awards.AwardWith(fast.Id, solvedWithinDayOne);
                    }
                }
            }
        }

        private void AwardFirstSolvedOnLevel(Awards awards, IEnumerable<Challenge> challenges, IEnumerable<ChallengeRanklist> challengeRanklists)
        {
            foreach (var ranklist in challengeRanklists)
            {
                var entries = ranklist.Submitters.Where(x => x.Points > 0).ToList();
                if (entries.Count > 0)
                {
                    var firstSubmitter = entries.OrderBy(x => x.Date).First();
                    awards.AwardWith(firstSubmitter.Id, firstSolvedChallenge);

                    var languages = entries.GroupBy(x => x.Language);
                    foreach (var language in languages)
                    {
                        var firstSubmitterPerLang = language.OrderBy(x => x.Date).First();
                        if (firstSubmitterPerLang.Id != firstSubmitter.Id)
                        {
                            awards.AwardWith(firstSubmitterPerLang.Id, firstSolvedLanguage);
                        }
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
                    awards.AwardWith(submitter.Name, fiftyWorkingSubmissions);
                }

                if (count >= 25)
                {
                    awards.AwardWith(submitter.Name, twentyFiveWorkingSubmissions);
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
                        awards.AwardWith(history.Id, fiveWorkingSubmissionsInRow);
                    }

                    if (challenges.Count >= 20)
                    {
                        awards.AwardWith(history.Id, twentySubmissionsInRow);
                        break;
                    }
                }
            }
        }

        public void AwardFirstSubmission(Awards awards, GlobalRanklist globals)
        {
            foreach (var submitter in globals.Submitters.Where(x => x.SubmissionCount > 0))
            {
                awards.AwardWith(submitter.Id, firstSubmission);
            }
        }

        public void AwardGettingOld(Awards awards, IEnumerable<IMember> members)
        {
            var sorted = members.OrderBy(x => x.DateOfEntry).ToList();
            for (var i = 0; i < sorted.Count - 4; i++)
            {
                awards.AwardWith(sorted[i].Id, gettingOld);
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
                        awards.AwardWith(submitter.Id, allLanguagesSolved);
                    }
                }
            }
        }
    }
}
