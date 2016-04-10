using System.IO;
using System.Linq;
using System.Threading;
using PatchKit.Data.Remote;
using PatchKit.Utilities;

namespace PatchKit.Data.Local
{
    /// <summary>
    /// Local data.
    /// </summary>
    internal class LocalApplication
    {
        /// <summary>
        /// Cache storing local data variables.
        /// </summary>
        public LocalCache Cache { get; private set; }

        /// <summary>
        /// Path to the local data.
        /// </summary>
        public readonly string Path;

        /// <summary>
        /// Application name. Used to separate data between multiple applications.
        /// </summary>
        public readonly string AppName;



        private readonly string _cachePath;

        private void RecreateCache()
        {
            Cache = new LocalCache(_cachePath);
        }

        public LocalApplication(string path, string appName)
        {
            Path = path;
            AppName = appName;
            _cachePath = System.IO.Path.Combine(Path, "patcher_cache.json");
            RecreateCache();
        }

        public bool IsUpToDate(int currentlyAvailableVersion)
        {
            return Cache.GetCommonVersion() == currentlyAvailableVersion;
        }

        /// <summary>
        /// Runs through all the application files and checks if 1) files are ther, 2) files has
        /// the same hash as required. Returns false if something is wrong.
        /// </summary>
        /// <param name="contentSummary"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        public bool CheckFilesConsistency(RemoteContentSummary contentSummary, CancellationToken cancellationToken)
        {
            foreach (var summaryFile in contentSummary.Files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string absolutePath = GetFilePath(summaryFile.Path);
                if (!File.Exists(absolutePath))
                {
                    return false;
                }

                string fileHash = HashUtilities.ComputeFileHash(absolutePath);
                if (!fileHash.Equals(summaryFile.Hash))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Clears local data (as well as cache).
        /// </summary>
        public void Clear(CancellationToken cancellationToken)
        {
            foreach (var fileName in Cache.GetFileNames().ToArray())
            {
                cancellationToken.ThrowIfCancellationRequested();
                ClearFile(fileName);
            }

            if (System.IO.File.Exists(_cachePath))
            {
                System.IO.File.Delete(_cachePath);
            }

            RecreateCache();
        }

        /// <summary>
        /// Clears data (as well as cache) about specified file.
        /// </summary>
        public void ClearFile(string fileName)
        {
            string filePath = GetFilePath(fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            Cache.ClearFileVersion(fileName);
        }

        /// <summary>
        /// Returns absolute path to specified file (placed in local data).
        /// </summary>
        public string GetFilePath(string fileName)
        {
            return System.IO.Path.Combine(Path, fileName);
        }
    }
}
