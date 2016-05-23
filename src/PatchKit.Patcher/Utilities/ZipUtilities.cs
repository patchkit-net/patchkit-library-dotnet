using System.IO;
using System.Threading;
using Ionic.Zip;

namespace PatchKit.Patcher.Utilities
{
    internal static class ZipUtilities
    {
        public delegate void FileUnzipStartHandler(string filePath, string fileName, float progress);

        public delegate void FileUnzipEndHandler(string filePath, string fileName, float progress);

        public static void Unzip(string packageFilePath, string destinationDirectoryPath)
        {
            Unzip(packageFilePath, destinationDirectoryPath, null, null, new CancellationToken());
        }

        /*
         * TODO: Można użyć streama zamaist packagePath - dzięki temu równolegle będzie miało miejsce pobieranie i rozpakowywanie. 
         */
        public static void Unzip(string packageFilePath, string destinationDirectoryPath, FileUnzipStartHandler onFileUnzipStart, FileUnzipEndHandler onFileUnzipEnd, CancellationToken cancelToken)
        {
            using (var zip = ZipFile.Read(packageFilePath))
            {
                int entryCounter = 0;

                foreach (ZipEntry zipEntry in zip)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    string fileDestinationPath = Path.Combine(destinationDirectoryPath, zipEntry.FileName);
                    if (!zipEntry.IsDirectory && onFileUnzipStart != null)
                    {
                        onFileUnzipStart(fileDestinationPath, zipEntry.FileName, entryCounter / (float)zip.Count);
                    }

                    zipEntry.Extract(destinationDirectoryPath, ExtractExistingFileAction.OverwriteSilently);
                    entryCounter++;

                    if (!zipEntry.IsDirectory && onFileUnzipEnd != null)
                    {
                        onFileUnzipEnd(fileDestinationPath, zipEntry.FileName, entryCounter / (float)zip.Count);
                    }
                }
            }
        }
    }
}
