using SubmissionEvaluation.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SubmissionEvaluation.Shared.Models
{
    public class GroupMemberships<T> where T: IMember
    {
        public string GroupName { get; set; }
        public List<T> Members { get; set; } = new List<T>();
    }
}
