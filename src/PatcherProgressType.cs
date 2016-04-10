namespace PatchKit
{
    public enum PatcherProgressType
    {
        /// <summary>
        /// Patching has been started.
        /// </summary>
        Started,
        /// <summary>
        /// Checking local version.
        /// </summary>
        Checking,
        /// <summary>
        /// Downloading data.
        /// </summary>
        Downloading,
        /// <summary>
        /// Unpacking downloaded data.
        /// </summary>
        Unpacking,
        /// <summary>
        /// Patching data.
        /// </summary>
        Patching,
        /// <summary>
        /// Patcher has failed.
        /// </summary>
        Failed,
        /// <summary>
        /// Patcher has been canceled.
        /// </summary>
        Canceled,
        /// <summary>
        /// Patcher has succeed.
        /// </summary>
        Succeed
    }
}