using SubmissionEvaluation.Shared.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubmissionEvaluation.Shared.Classes
{
    public class PermissionHelper
    {
        static public bool CheckPermissions(Actions action, string area, Permissions permissions, Restriction accessibles = Restriction.NONE, string id = null)
        {
            List<string> checkingPermissions;
            List<string> accessibleList = null;
            switch (action)
            {
                case Actions.CREATE: checkingPermissions = permissions.CreatePermissions; break;
                case Actions.VIEW: checkingPermissions = permissions.ViewPermissions; break;
                case Actions.EDIT: checkingPermissions = permissions.EditPermissions; break;
                default: checkingPermissions = new List<string>(); break;
            }
            if (accessibles != Restriction.NONE)
            {
                switch (accessibles)
                {
                    case Restriction.CHALLENGES: accessibleList = permissions.ChallengesAccessible; break;
                    case Restriction.GROUPS: accessibleList = permissions.GroupsAccessible; break;
                    case Restriction.BUNDLES: accessibleList = permissions.BundlesAccessible; break;
                    case Restriction.MEMBERS: accessibleList = permissions.MembersAccessible; break;
                }
            }
            return permissions.isAdmin || (checkingPermissions.Contains(area) && (accessibleList == null || accessibleList.Contains(id)));
        }
    }
}
