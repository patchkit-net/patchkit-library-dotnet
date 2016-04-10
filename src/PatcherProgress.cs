using System;

namespace PatchKit
{
    /// <summary>
    /// Information about patcher progress.
    /// </summary>
    public struct PatcherProgress
    {
        /// <summary>
        /// Type of progress.
        /// </summary>
        public PatcherProgressType Type;

        /// <summary>
        /// Currently processed file's name.
        /// </summary>
        public string FileName;

        /// <summary>
        /// Currently processed file's path.
        /// </summary>
        public string FilePath;

        /// <summary>
        /// Progress value between 0 and 1.
        /// </summary>
        public float Progress;

        /// <summary>
        /// Exception that caused the failure.
        /// </summary>
        public Exception Exception;
    }
}
