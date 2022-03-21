namespace SubmissionEvaluation.Contracts.Data
{
    public enum RatingMethod
    {
        Fixed,
        Score,
        ExecTime, //Naming with underscore is important for parsing from text-files. All Challenge files, the challenge stat file and the settings.json would need to be changed.
        SubmissionTime
    }
}
