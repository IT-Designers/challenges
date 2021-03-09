namespace SubmissionEvaluation.Server.Classes.Messages
{
    public static class ErrorMessages
    {
        public static string WrongUserPassword => nameof(WrongUserPassword);
        public static string InvalidFile => nameof(InvalidFile);
        public static string InvalidJekyllUser => nameof(InvalidJekyllUser);
        public static string GenericError => nameof(GenericError);
        public static string ChallengeNameSpaces => nameof(ChallengeNameSpaces);
        public static string NoPermission => nameof(NoPermission);
        public static string ConfirmDeleteChallenge => nameof(ConfirmDeleteChallenge);
        public static string InvalidMarkdown => nameof(InvalidMarkdown);
        public static string NoCompilerFound => nameof(NoCompilerFound);
        public static string UploadError => nameof(UploadError);
        public static string IdError => nameof(IdError);
        public static string IdAlreadyExists => nameof(IdAlreadyExists);
        public static string PreviousChallengesNotSolved => nameof(PreviousChallengesNotSolved);
        public static string MissingName => nameof(MissingName);
        public static string MissingPassword => nameof(MissingPassword);
        public static string UserCreateFailed => nameof(UserCreateFailed);
        public static string MissingMail => nameof(MissingMail);
        public static string ActivationNeeded => nameof(ActivationNeeded);
        public static string UserNotFound => nameof(UserNotFound);
        public static string NoReview => nameof(NoReview);
        public static string ReviewGenerationFailed => nameof(ReviewGenerationFailed);
        public static string ReviewIsCurrentlyGenerated => nameof(ReviewIsCurrentlyGenerated);
        public static string MustJoinGroup => nameof(MustJoinGroup);
        public static string ChallengeLockedForUser => nameof(ChallengeLockedForUser);
        public static string WrongFormat => nameof(WrongFormat);
    }
}
