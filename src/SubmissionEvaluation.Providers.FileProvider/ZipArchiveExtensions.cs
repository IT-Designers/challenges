using System.IO;
using System.IO.Compression;
using System.Linq;

namespace SubmissionEvaluation.Providers.FileProvider
{
    public static class ZipArchiveExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string dest, bool overwrite)
        {
            if (!overwrite)
            {
                archive.ExtractToDirectory(dest);
                return;
            }

            foreach (var file in archive.Entries.OrderBy(x => x.LastWriteTime))
            {
                var completeFileName = Path.Combine(dest, file.FullName);
                var directory = Path.GetDirectoryName(completeFileName);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!string.IsNullOrEmpty(file.Name))
                {
                    file.ExtractToFile(completeFileName, true);
                }
            }
        }
    }
}
