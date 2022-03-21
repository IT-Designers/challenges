using SubmissionEvaluation.Shared.Models.Permissions;

namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class PermissionSettings
    {
        public Permissions GlobalAdminPermissions { get; } = new Permissions {IsAdmin = true};
        public Permissions Creator { get; } = new Permissions();
        public Permissions GroupAdmin { get; } = new Permissions();
        public Permissions GroupReviewer { get; } = new Permissions();
    }
}
