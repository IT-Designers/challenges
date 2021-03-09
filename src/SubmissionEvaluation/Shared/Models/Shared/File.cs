namespace SubmissionEvaluation.Shared.Models.Shared
{
    public class File
    {
        public File()
        {
            Type = Type ?? "file";
        }

        public File(File file)
        {
            Name = file.Name.Replace("/", Folder.pathSeperator).Replace("\\\\", Folder.pathSeperator);
            OriginalName = file.Name;
            IsFolder = file.IsFolder;
            IsDelete = file.IsDelete;
            LastModified = file.LastModified;
            Type = file.Type;
            Path = file.Path;
        }

        //This is the full path name.
        //For tests this is the name of the file for the compiler.
        public string Name { get; set; }

        //This is the original file name before first saving. Afterwards it will be the full path name before editing files name.
        public string OriginalName { get; set; }
        public bool IsFolder { get; set; }
        public bool IsDelete { get; set; }

        public string LastModified { get; set; }

        //Defining the mime-type.
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
