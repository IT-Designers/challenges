namespace Challenge.Deploy
{
    public class CopyOperation
    {
        public string Name { get; set; }
        public string[] IncludeFiles { get; set; }
        public string[] ExcludeDirs { get; set; }
    }
}
