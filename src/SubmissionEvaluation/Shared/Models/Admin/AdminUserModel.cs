using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models.Admin
{
    public class AdminUserModel<T> : GenericModel where T : IMember
    {
        public List<T> Members { get; set; }
    }

    public class AdminGroupsModel<T> : GenericModel where T : IGroup
    {
        public IEnumerable<T> Groups { get; set; }
    }
}
