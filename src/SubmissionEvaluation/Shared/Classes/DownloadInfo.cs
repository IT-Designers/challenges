namespace SubmissionEvaluation.Shared.Classes
{
    //Containing the data to be downloaded and otherwise a errormessage.
    public class DownloadInfo
    {
        public DownloadInfo()
        {
        }

        public DownloadInfo(string message)
        {
            ErrorMessage = message;
        }

        public DownloadInfo(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; set; }
        public string ErrorMessage { get; set; }
    }
}
