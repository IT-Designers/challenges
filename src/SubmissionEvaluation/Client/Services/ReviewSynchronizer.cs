using System.Collections.Generic;
using System.Linq;
using SubmissionEvaluation.Client.Shared.Models;
using SubmissionEvaluation.Contracts.Data.Review;
using SubmissionEvaluation.Shared.Models.Review;

namespace SubmissionEvaluation.Client.Services
{
    public class ReviewSynchronizer
    {
        public delegate void dataChanged();

        public delegate void fileChanged();

        public FileReviewCommentsAssoziater CurrentAssoziation { get; set; }

        public List<FileReviewCommentsAssoziater> AllFilesWithComments { get; set; } = new List<FileReviewCommentsAssoziater>();

        public event dataChanged SomeDataChanged;
        public event fileChanged FileHasChanged;

        public void InvokeEvent()
        {
            SomeDataChanged?.Invoke();
        }

        public void InvokeFileChangeEvent()
        {
            FileHasChanged?.Invoke();
        }

        public void SetCurrentAssoziation(string name)
        {
            CurrentAssoziation = AllFilesWithComments.Where(x => x.SourceFile.Name.Equals(name)).FirstOrDefault();
        }

        public void SetAllFilesWithComments(List<ReviewCodeComments> comments, List<ReviewFile> files)
        {
            foreach (var file in files)
            {
                AllFilesWithComments.Add(new FileReviewCommentsAssoziater(comments.Where(x => x.FileName.Equals(file.Name)).FirstOrDefault(), file));
            }
        }
    }
}
