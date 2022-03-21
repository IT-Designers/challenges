using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Client.Shared.Models;
using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Shared.Models.Review;

namespace SubmissionEvaluation.Client.Services
{
    public class ReviewSynchronizer
    {
        public delegate void DataChanged();

        public delegate void FileChanged();

        public string CurrentIssueId { get; set; }

        public string CurrentIssueTitle { get; set; }

        public FileReviewCommentsAssoziater CurrentAssociation { get; set; }

        public List<FileReviewCommentsAssoziater> AllFilesWithComments { get; set; } = new List<FileReviewCommentsAssoziater>();

        public event DataChanged SomeDataChanged;
        public event FileChanged FileHasChanged;

        public void InvokeEvent()
        {
            SomeDataChanged?.Invoke();
        }

        public void InvokeFileChangeEvent()
        {
            FileHasChanged?.Invoke();
        }

        public void clear()
        {
            AllFilesWithComments = new List<FileReviewCommentsAssoziater>();
            CurrentAssociation =  null;
        }

        public void SetCurrentAssoziation(string name)
        {
            CurrentAssociation = AllFilesWithComments.FirstOrDefault(x => x.SourceFile.Name.Equals(name));
        }

        public void SetAllFilesWithComments(List<ReviewCodeComments> comments, List<ReviewFile> files)
        {
            foreach (var file in files)
            {
                AllFilesWithComments.Add(new FileReviewCommentsAssoziater(comments.FirstOrDefault(x => x.FileName.Equals(file.Name)), file));
            }
        }
    }
}
