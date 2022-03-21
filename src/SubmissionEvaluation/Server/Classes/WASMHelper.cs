using System.Collections.Generic;
using System.Collections.ObjectModel;
using SubmissionEvaluation.Contracts.Data;
using SubmissionEvaluation.Shared.Classes.Config;

namespace SubmissionEvaluation.Server.Classes
{
    //This class helps the server to convert Dictionaries, that do not use a string as key to such that due,
    //since WASM does not support the objects otherwise.
    public class WasmHelper
    {
        private WasmHelper()
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
            {RatingMethod.ExecTime, "ExecTime"},
            {RatingMethod.SubmissionTime, "SubmissionTime"}
        });

        //Converts RatingMethods into WASM-supported Dictionary
        public Dictionary<string, RatingMethodConfig> RatingMethodsConverted { get; } = new Dictionary<string, RatingMethodConfig>();

        public static WasmHelper Helper { get; } = new WasmHelper();

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
