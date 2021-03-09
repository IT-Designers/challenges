using System.Collections.Generic;
using System.Collections.ObjectModel;
using SubmissionEvaluation.Classes.Config;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server.Classes
{
    //This class helps the server to convert Dictionaries, that do not use a string as key to such that due,
    //since WASM does not support the objects otherwise.
    public class WASMHelper
    {
        private WASMHelper()
        {
            foreach (var entry in Settings.Customization.RatingMethods)
            {
                if (Converter.ContainsKey(entry.Key) && Converter[entry.Key] != null)
                {
                    RatingMethodsConverted.Add(Converter[entry.Key], entry.Value);
                }
            }
        }

        public ReadOnlyDictionary<RatingMethod, string> Converter { get; } = new ReadOnlyDictionary<RatingMethod, string>(new Dictionary<RatingMethod, string>
        {
            {RatingMethod.Fixed, "Fixed"},
            {RatingMethod.Score, "Score"},
            {RatingMethod.Exec_Time, "Exec_Time"},
            {RatingMethod.Submission_Time, "Submission_Time"}
        });

        //Converts RatingMethods into WASM-supported Dictionary
        public Dictionary<string, RatingMethodConfig> RatingMethodsConverted { get; } = new Dictionary<string, RatingMethodConfig>();

        public static WASMHelper helper { get; } = new WASMHelper();

        public RatingMethodConfig ValueRatingMethod(RatingMethod ratingId)
        {
            //if (string.IsNullOrWhiteSpace(ratingId)) return Settings.Customization.RatingMethods["fixed"];
            if (Settings.Customization.RatingMethods.TryGetValue(ratingId, out var method))
            {
                return method;
            }

            return new RatingMethodConfig {Color = "FF0000", Title = "Missing: " + ratingId};
        }
    }
}
