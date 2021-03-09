using System.Collections.Generic;
using SubmissionEvaluation.Contracts.Data;

namespace SubmissionEvaluation.Shared.Models
{
    public class CategoryListModel<T> where T : IMember
    {
        public string Category { get; set; }
        public IEnumerable<CategoryListEntryModel> Entries { get; set; }
        public T Member { get; set; }
    }
}
