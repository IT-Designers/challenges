using System.Collections.Generic;
using SubmissionEvaluation.Shared.Models.Permissions;

namespace SubmissionEvaluation.Shared.Classes
{
    public static class PermissionHelper
    {
        public static bool CheckPermissions(Actions action, string area, Permissions permissions, Restriction accessibles = Restriction.None, string id = null)
        {
            if (permissions == null) { return false; }

            List<string> checkingPermissions;
            switch (action)
            {
                case Actions.Create:
                    checkingPermissions = permissions.CreatePermissions;
                    break;
                case Actions.View:
                    checkingPermissions = permissions.ViewPermissions;
                    break;
                case Actions.Edit:
                    checkingPermissions = permissions.EditPermissions;
                    break;
                default:
                    checkingPermissions = new List<string>();
                    break;
            }

            List<string> accessibleList;
            switch (accessibles)
            {
                case Restriction.Challenges:
                    accessibleList = permissions.ChallengesAccessible;
                    break;
                case Restriction.Groups:
                    accessibleList = permissions.GroupsAccessible;
                    break;
                case Restriction.Bundles:
                    accessibleList = permissions.BundlesAccessible;
                    break;
                case Restriction.Members:
                    accessibleList = permissions.MembersAccessible;
                    break;
                case Restriction.None:
                    accessibleList = new List<string>();
                    break;
                default:
                    accessibleList = new List<string>();
                    break;
            }

            return permissions.IsAdmin || checkingPermissions.Contains(area) && (accessibleList.Count == 0 || accessibleList.Contains(id));
        }
    }
}
