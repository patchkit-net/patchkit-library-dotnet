namespace PatchKit.Patcher.Downloader
{
    public enum DownloaderProgressType
    {
        /// <summary>
        /// Download has been started.
        /// </summary>
        Started,
        /// <summary>
        /// Data is being downloaded.
        /// </summary>
        Downloading,
        /// <summary>
        /// Downloader has been canceled.
        /// </summary>
        Canceled,
        /// <summary>
        /// Downloader has failed.
        /// </summary>
        Failed,
        /// <summary>
        /// Downloader has succeed.
        /// </summary>
        Succeed
    }
}