using System;

namespace PatchKit.Patcher.Downloader
{
    /// <summary>
    /// Information about downloader progress.
    /// </summary>
    public struct DownloaderProgress
    {
        /// <summary>
        /// Type of progress.
        /// </summary>
        public DownloaderProgressType Type;

        /// <summary>
        /// Downloaded destination file's path.
        /// </summary>
        public string DestinationFilePath;

        /// <summary>
        /// Progress value between 0 and 1.
        /// </summary>
        public float Progress;

        /// <summary>
        /// Amount of downloaded bytes.
        /// </summary>
        public long DownloadedBytesCount;

        /// <summary>
        /// Amount of total bytes to download.
        /// </summary>
        public long TotalBytesCount;

        /// <summary>
        /// Exception that caused the failure.
        /// </summary>
        public Exception Exception;
    }
}
