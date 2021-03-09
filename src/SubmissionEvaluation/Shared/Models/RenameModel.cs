using System.ComponentModel.DataAnnotations;

namespace SubmissionEvaluation.Shared.Models
{
    public class RenameModel : GenericModel
    {
        [Required] public string Name { get; set; }

        [Required] public string NewName { get; set; }
    }
}
