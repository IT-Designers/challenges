using System.Collections.Generic;

namespace SubmissionEvaluation.Shared.Models.Shared
{
    public class Folder : DetailedInputFile
    {
        //This is the seperator, cause no one will ever use it.
        public static readonly string PathSeperator = "§";

        public Folder()
        {
            FilesInFolder = FilesInFolder ?? new List<File>();
            IsFolder = true;
            Content = new byte[0];
            NewFilesInFolder = NewFilesInFolder ?? new List<DetailedInputFile>();
            Type = "folder";
        }

        public bool IsExpanded { get; set; }
        public List<File> FilesInFolder { get; set; }
        public List<DetailedInputFile> NewFilesInFolder { get; set; }
    }
}
