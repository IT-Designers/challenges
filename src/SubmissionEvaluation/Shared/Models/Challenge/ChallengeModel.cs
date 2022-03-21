using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Models.Shared;
using SubmissionEvaluation.Shared.Models.Test;
using Group = SubmissionEvaluation.Contracts.ClientPocos.Group;

namespace SubmissionEvaluation.Shared.Models.Challenge
{
    public class ChallengeModel : ChallengeInfoModel
    {
        public ChallengeModel(IChallenge c)
        {
            AuthorId = c.AuthorId;
            Category = c.Category;
            Date = c.Date;
            Description = c.Description;
            Id = c.Id;
            IsDraft = c.IsDraft;
            Languages = c.Languages;
            LastEdit = c.LastEdit;
            LastEditorId = c.LastEditorId;
            RatingMethod = c.RatingMethod;
            Source = c.Source;
            Title = c.Title;
            HasChallengeError = c.State.HasError;
            ChallengeErrorDescription = c.State.ErrorDescription;
            FeasibilityIndex = c.State.FeasibilityIndex;
        }

        public ChallengeModel()
        {
        }

        [Required(ErrorMessage = "Du musst eine Beschreibung für diese Challenge angeben!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Du musst einen Titel für diese Challenge angeben!")]
        public bool IsDraft { get; set; }

        public string SourceType { get; set; }
        public string SourceUrl { get; set; }

        [JsonIgnore]
        public string Source
        {
            get
            {
                if (SourceType?.Equals("own", StringComparison.CurrentCultureIgnoreCase) == true)
                {
                    return "none";
                }

                return SourceUrl;
            }
            set
            {
                if (value == "none")
                {
                    SourceUrl = "";
                    SourceType = "own";
                }
                else
                {
                    SourceUrl = value;
                    SourceType = "other";
                }
            }
        }

        public DateTime Date { get; set; }
        public List<File> Files { get; set; }
        public bool HasChallengeError { get; set; }
        public string ChallengeErrorDescription { get; set; }
        public bool IsGettingCreated { get; set; } = false;
        public List<string> Languages { get; set; }
        public string IncludeTests { get; set; }
        public List<ChallengeTest> Tests { get; set; }

        public Dictionary<string, string> RatingMethods { get; set; }
        public Dictionary<string, string> Categories { get; set; }
        public List<string> KnownLanguages { get; set; }
        public Dictionary<string, string> SourceTypes { get; set; }
        public int FeasibilityIndex { get; set; }
        public List<Group> Groups { get; set; }
        public RatingPoints Points { get; set; }
        public string DependsOn { get; set; }
        public DateTime LastEdit { get; set; }
        public string LearningFocus { get; set; }
    }
}
