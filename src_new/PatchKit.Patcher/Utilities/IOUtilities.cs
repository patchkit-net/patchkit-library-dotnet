using System.IO;
using System.Threading;

namespace PatchKit.Patcher.Utilities
{
    internal static class IOUtilities
    {
        internal const int CopyFileBufferSize = 1024;

        internal static void CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite, CancellationToken cancelToken)
        {
            using (var sourceReader = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                using (var destinationReader = new FileStream(destinationFilePath, overwrite ? FileMode.Create : FileMode.CreateNew, FileAccess.Write))
                {
                    sourceReader.CopyToAsync(destinationReader, CopyFileBufferSize, cancelToken).Wait(cancelToken);
                }
            }
        }
    }
}
