using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace PatchKit.Patcher.Utilities
{
    internal class PatchException : Exception
    {
        public PatchException(string message) : base(message)
        {
        }

        public PatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    internal static class PatchUtilities
    {
        public static void Patch(string originalFilePath, string patchFilePath, CancellationToken cancelToken)
        {
            string outputFilePath = originalFilePath + ".patched";

            string rdiffPath = Path.Combine(PathUtilities.AssemblyDirectory, "rdiff.exe");

            if (!File.Exists(rdiffPath))
            {
                throw new PatchException("Cannot find rdiff executable.");
            }

            string rdiffArguments = string.Format("patch \"{0}\" \"{1}\" \"{2}\"", originalFilePath, patchFilePath,
                outputFilePath);

            Process rdiffProcess = new Process
            {
                StartInfo = new ProcessStartInfo(rdiffPath, rdiffArguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                }
            };

            if (!rdiffProcess.Start())
            {
                throw new PatchException("Failed to start rdiff exectuable.");
            }

            cancelToken.Register(() => rdiffProcess.Kill());

            rdiffProcess.WaitForExit();

            cancelToken.ThrowIfCancellationRequested();

            if (!File.Exists(outputFilePath))
            {
                throw new PatchException("Unable to patch with rdiff.");
            }

            IOUtilities.CopyFile(outputFilePath, originalFilePath, true, cancelToken);
            File.Delete(outputFilePath);
        }
    }
}
