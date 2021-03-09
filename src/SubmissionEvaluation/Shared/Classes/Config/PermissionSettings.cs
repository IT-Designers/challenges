using SubmissionEvaluation.Shared.Models.Permissions;

namespace SubmissionEvaluation.Shared.Classes.Config
{
    public class PermissionSettings
    {
        public Permissions GlobalAdminPermissions { get; } = new Permissions {isAdmin = true};
        public Permissions CreatorPermissions { get; } = new Permissions();
        public Permissions GroupAdminPermissions { get; } = new Permissions();
        public Permissions GroupReviewerPermissions { get; } = new Permissions();
    }
}
