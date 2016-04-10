using System;

namespace PatchKit
{
    /// <summary>
    /// Connection settings for patcher.
    /// </summary>
    public struct PatcherSettings
    {
        /// <summary>
        /// URL to patching service.
        /// </summary>
        public Uri ServiceURL;

        /// <summary>
        /// Secret key for application.
        /// </summary>
        public string SecretKey;
    }
}
